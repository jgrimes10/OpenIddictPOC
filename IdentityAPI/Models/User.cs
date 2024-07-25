using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAPI.Models
{
    [Table(nameof(User))]
    public class User : IdentityUser
    {
        [Required]
        public MfaMethod MfaMethod { get; set; } = MfaMethod.None;
        
        public string? AuthenticatorKey { get; set; }

        public List<UserRole> UserRoles { get; set; }
    }
}
