using System.ComponentModel.DataAnnotations;

namespace IdentityAPI.Models
{
    public class UserEnableAuthenticatorModel
    {
        [Required]
        public required string Username { get; set; }
    }
}
