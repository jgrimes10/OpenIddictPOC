using System.ComponentModel.DataAnnotations;

namespace IdentityAPI.Models;

public class Verify2FAEnableRequest
{
    [Required] public required string Email { get; set; }
    [Required] public required string Code { get; set; }
}