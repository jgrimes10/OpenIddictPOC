using System.Security.Claims;
using IdentityAPI.Models;
using IdentityAPI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace IdentityAPI.Controllers
{
    /// <summary>
    /// Handles authentication-related actions, including token generation and user registration.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly MfaService _mfaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager for handling user-related operations.</param>
        /// <param name="signInManager">The sign-in manager for handling user sign-in operations.</param>
        /// <param name="mfaService">The service for handling MFA operations.</param>
        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, MfaService mfaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mfaService = mfaService;
        }

        /// <summary>
        /// Handles token generation requests for various grant types including password and refresh token grants.
        /// </summary>
        /// <returns>An action result that represents the outcome of the token generation process.</returns>
        [HttpPost("~/connect/token")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var oidcRequest = HttpContext.GetOpenIddictServerRequest();
            if (oidcRequest == null)
            {
                throw new Exception("oidcRequest is null");
            }
            
            if (oidcRequest.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(oidcRequest.Username);
                if (user == null)
                {
                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "The username/password is invalid."
                    });
                }
                
                // Ensure the user is allowed to sign in.
                //if (!await _signInManager.CanSignInAsync(user))
                //{
                    //return BadRequest(new OpenIddictResponse
                    //{
                        //Error = OpenIddictConstants.Errors.InvalidGrant,
                        //ErrorDescription = "The specified user is not allowed to sign in."
                    //});
                //}
                
                // Ensure the password is valid.
                if (!await _userManager.CheckPasswordAsync(user, oidcRequest.Password))
                {
                    if (_userManager.SupportsUserLockout)
                    {
                        await _userManager.AccessFailedAsync(user);
                    }

                    return BadRequest(new OpenIddictResponse
                    {
                        Error = OpenIddictConstants.Errors.InvalidGrant,
                        ErrorDescription = "The username/password is invalid."
                    });
                }
                
                // Reject the token request if two-factor authentication has been enabled by the user.
                if (_userManager.SupportsUserTwoFactor && await _userManager.GetTwoFactorEnabledAsync(user))
                {
                    var mfaMethod = await _mfaService.GetMfaMethodAsync(user);
                    
                    // User tried signing in without a code AND they have mfa turned on.
                    if (string.IsNullOrWhiteSpace(oidcRequest.Code))
                    {
                        // Generate a token to return for the 2FA.
                        var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider,
                            "TwoFactor");

                        switch (mfaMethod)
                        {
                            case MfaMethod.Sms:
                                // The user has SMS as their MFA method, so we need to generate a code to send to them.
                                var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                                
                                // TODO: Don't send the code back here! Eventually it will be sent via Email/SMS.
                                return Ok(new { Requires2FA = true, Token = token, Code = code });
                            
                            case MfaMethod.Authenticator:
                                // The user has an authenticator as their MFA method, just send back the token.
                                return Ok(new { Requires2FA = true, Token = token });
                            
                            case MfaMethod.None:
                                // Something has gone wrong... The user has MFA turned on but no method of MFA...
                                throw new Exception("Mfa method mismatch - Login attempt.");
                        }
                    }

                    // The user is signing in with a code in the request, now we need to check it.
                    switch (mfaMethod)
                    {
                        case MfaMethod.Sms:
                            if (!await VerifySmsToken(user, oidcRequest.Code))
                            {
                                return BadRequest(new OpenIddictResponse
                                {
                                     Error = OpenIddictConstants.Errors.InvalidRequest,
                                     ErrorDescription = "Invalid SMS code."
                                });       
                            }
                            break;
                            
                        case MfaMethod.Authenticator:
                            if (!await VerifyAuthorizationToken(user, oidcRequest.Code))
                            {
                                return BadRequest(new OpenIddictResponse
                                {
                                    Error = OpenIddictConstants.Errors.InvalidRequest,
                                    ErrorDescription = "Invalid authorization code."
                                });
                            }
                            break;
                        
                        case MfaMethod.None:
                            // Again, something has gone wrong, and we should never have mfa turned on
                            // without a type of mfa selected.
                            throw new Exception("Mfa method mismatch - login verification.");
                    }
                    //if (!await Verify2FaCodeAsync(user, oidcRequest.Token, oidcRequest.Code))
                    //{
                        //return BadRequest(new OpenIddictResponse
                        //{
                            //Error = OpenIddictConstants.Errors.InvalidRequest,
                            //ErrorDescription = "Invalid mfa code."
                        //});
                    //}
                }

                //if (_userManager.SupportsUserLockout)
                //{
                    //await _userManager.ResetAccessFailedCountAsync(user);
                //}
                
                return await TokensForPasswordGrantType(oidcRequest);
            }

            if (oidcRequest.IsRefreshTokenGrantType())
            {
                // Return tokens for refresh token flow.
            }

            if (oidcRequest.GrantType == "custom_flow_name")
            {
                // Return tokens for custom flow.
            }

            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedGrantType
            });
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="model">The registration details.</param>
        /// <returns>An action result indicating the outcome of the registration process.</returns>
        [HttpPost("~/register")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.EmailAddress,
                UserRoles = new List<UserRole>()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully!" });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
        }

        [HttpPost("~/forgot-password")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(forgotPasswordModel.Username);
                if (user == null)
                {
                    return Ok(); // Do not reveal that the user does not exist.
                }
                
                // Generate a password reset token.
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Send email with password reset link.
                return Ok(new { token });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("~/reset-password")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(resetPasswordModel.Username);
            if (user == null)
            {
                // Return a generic message to avoid revealing user information.
                return Ok(new { message = "Password reset successful." });
            }

            var decodedToken = Uri.UnescapeDataString(resetPasswordModel.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordModel.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password reset successful." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("~/enable-authenticator")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> EnableAuthenticator([FromForm] UserEnableAuthenticatorModel enableAuthenticatorModel)
        {
            var user = await _userManager.FindByNameAsync(enableAuthenticatorModel.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new
            {
                AuthenticatorKey = authenticatorKey,
                QRCodeUri = GenerateQrCodeUri(user.Email ?? throw new InvalidOperationException(), authenticatorKey ?? throw new Exception("Authenticator key null"))
            };

            return Ok(model);
        }

        [HttpPost("~/disable-authenticator")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> DisableAuthenticator(
            [FromForm] UserEnableAuthenticatorModel enableAuthenticatorModel)
        {
            var user = await _userManager.FindByNameAsync(enableAuthenticatorModel.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _mfaService.DisableMfaAsync(user);
            return Ok("2FA has been disabled.");
        }

        [HttpPost("~/confirm-authenticator")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> Confirm2FaToEnable([FromForm] VerifyAuthenticatorToEnableRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            // Check if the given code is valid for this user.
            var valid = await VerifyAuthorizationToken(user, model.Code);

            if (!valid)
            {
                return Unauthorized("Invalid 2FA token.");
            }

            await _mfaService.SetMfaMethodAsync(user, MfaMethod.Authenticator);
            return Ok("2FA has been enabled.");
        }

        [HttpPost("~/enable-sms-mfa")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> EnableSmsMfa([FromForm] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            return Ok(new { code });
        }

        [HttpPost("~/disable-sms-mfa")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> DisableSmsMfa([FromForm] string username)
        {
            // Should we require the code to disable? As a security measure so an unintended user
            // of the device cannot disable 2FA. This may require "employee" intervention though
            // if the user changes their phone number and can no longer receive SMS messages.
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }

            await _mfaService.DisableMfaAsync(user);
            return Ok("SMS MFA has been disabled.");
        }

        [HttpPost("~/verify-sms-mfa")]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> ConfirmToEnableSmsMfa([FromForm] string username, [FromForm] string code)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }
            
            // Check if the provided code is valid.
            var isValid = await VerifySmsToken(user, code);
            if (!isValid)
            {
                return Unauthorized("Code is invalid.");
            }

            await _mfaService.SetMfaMethodAsync(user, MfaMethod.Sms);
            return Ok("SMS MFA has been enabled.");
        }

        private async Task<bool> Verify2FaCodeAsync(User user, string token, string code)
        {
            var is2faTokenValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "TwoFactor", token);
            // THIS IS WRONG, DONE FOR TESTING.
            if (is2faTokenValid)
            {
                return false;
            }

            var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);

            return await VerifyAuthorizationToken(user, verificationCode);
        }

        // TODO: Probably can combine the verify methods and pass in the auth type to determine which provider to use.
        private async Task<bool> VerifySmsToken(User user, string code)
        {
            var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is1FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user,
                TokenOptions.DefaultPhoneProvider, verificationCode);

            if (!is1FaTokenValid)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> VerifyAuthorizationToken(User user, string code)
        {
            var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is1FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is1FaTokenValid)
            {
                return false;
            }

            return true;
        }

        private string GenerateQrCodeUri(string email, string key)
        {
            return string.Format("otpauth://totp/{0}?secret={1}&issuer={2}&digits=6", email, key, "IdentityAPI");
        }

        /// <summary>
        /// Processes the token request for the password grant type by authenticiating the user with the provided username and password.
        /// If authentication is successful, generates and returns an access token along with any other requested information.
        /// </summary>
        /// <param name="request">The OpenIddict request containing the username and password submitted by the client.</param>
        /// <returns>An IActionResult that may contain the generated access token if authentication is successful, or an Unauthorized result if not.</returns>
        private async Task<IActionResult> TokensForPasswordGrantType(OpenIddictRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (signInResult.Succeeded)
            {
                var identity = new ClaimsIdentity(
                    TokenValidationParameters.DefaultAuthenticationType,
                    OpenIddictConstants.Claims.Name,
                    OpenIddictConstants.Claims.Role);
                
                identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString(), OpenIddictConstants.Destinations.AccessToken);
                identity.AddClaim(OpenIddictConstants.Claims.Username, user.UserName, OpenIddictConstants.Destinations.AccessToken);
                // Add more claims if necessary.

                foreach (var userRole in user.UserRoles)
                {
                    identity.AddClaim(OpenIddictConstants.Claims.Role, userRole.Role.NormalizedName, OpenIddictConstants.Destinations.AccessToken);
                }

                var claimsPrincipal = new ClaimsPrincipal(identity);
                claimsPrincipal.SetScopes(new string[]
                {
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile
                });

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else
            {
                return Unauthorized();
            }
        }

        private async Task<ClaimsPrincipal> CreatePrincipleAsync(User user, OpenIddictRequest request)
        {
            var principle = await _signInManager.CreateUserPrincipalAsync(user);

            principle.SetScopes(new[]
            {
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.OfflineAccess,
            }.Intersect(request.GetScopes()));

            return principle;
        }
    }
}
