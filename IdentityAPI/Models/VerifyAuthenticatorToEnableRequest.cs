using System.ComponentModel.DataAnnotations;

namespace IdentityAPI.Models;

public class VerifyAuthenticatorToEnableRequest
{
    [Required] public required string Username { get; set; }
    [Required] public required string Code { get; set; }
}