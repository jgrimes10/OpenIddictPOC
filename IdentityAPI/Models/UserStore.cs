using IdentityAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityAPI.Models
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>, IUserEmailStore<User>
    {
        private readonly ApplicationDbContext _context;

        public UserStore(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Username == normalizedUserName, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Set the password hash on the user object.
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetEmailAsync(User user, string? email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var user = _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .SingleOrDefaultAsync(u => u.EmailAddress.Equals(normalizedEmail), cancellationToken);
                
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while finding the user by email.", ex);
            }
        }

        public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailAddress)!;
        }

        public Task SetNormalizedEmailAsync(User user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailAddress = normalizedEmail;
            return Task.CompletedTask;
        }
    }
}
