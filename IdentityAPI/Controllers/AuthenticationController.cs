using System.Security.Claims;
using IdentityAPI.Models;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager for handling user-related operations.</param>
        /// <param name="signInManager">The sign-in manager for handling user sign-in operations.</param>
        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            if (oidcRequest.IsPasswordGrantType())
            {
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
                Username = model.Username,
                EmailAddress = model.EmailAddress,
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
                identity.AddClaim(OpenIddictConstants.Claims.Username, user.Username, OpenIddictConstants.Destinations.AccessToken);
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
    }
}
