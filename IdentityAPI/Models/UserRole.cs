using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityAPI.Models
{
    [Table(nameof(UserRole))]
    public class UserRole
    {
        [ForeignKey(nameof(IdentityAPI.Models.User.Id))]
        public Guid UserId { get; set; }
        required public User User { get; set; }

        [ForeignKey(nameof(IdentityAPI.Models.Role.Id))]
        public long RoleId { get; set; }
        required public Role Role { get; set; }
    }
}
