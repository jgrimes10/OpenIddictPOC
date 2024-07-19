using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityAPI.Models
{
    [Table(nameof(Role))]
    public class Role : IdentityRole<long>
    {
        required public List<UserRole> UserRoles { get; set; }
    }
}
