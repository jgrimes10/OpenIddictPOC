using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace IdentityAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="TestController"/> for testing purposes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// A test endpoint that returns the name of the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint requires the user to be authenticated using the configured OpenIddict validation scheme.
        /// It demonstrates how to access the user's identity and return a part of it.
        /// </remarks>
        /// <returns>An <see cref="IActionResult"/> containing the authenticated user's name.</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        public IActionResult Test()
        {
            var identity = HttpContext.User.Identity;

            return Ok(new { identity.Name });
        }
    }
}
