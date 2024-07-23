using IdentityAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityAPI.Models
{
    public class UserStore : 
        IUserPasswordStore<User>,
        IUserRoleStore<User>,
        IUserEmailStore<User>,
        IUserAuthenticatorKeyStore<User>,
        IUserTwoFactorStore<User>
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
            _context?.Dispose();
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
                .SingleOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
               // Ensure the cancellation token is not cancelled.
               cancellationToken.ThrowIfCancellationRequested();
   
               // Ensure the user object is not null
               if (user == null)
               {
                   throw new ArgumentNullException(nameof(user));
               }

               var userRoles = await _context.UserRoles
                   .Where(ur => ur.UserId == user.Id)
                   .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                   .ToListAsync(cancellationToken);

               return userRoles!;
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Return the user id.
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
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
            user.UserName = normalizedName;
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

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Attach the user entity to the context and mark it as modified.
            _context.Entry(user).State = EntityState.Modified;

            // Attempt to save changes to the database.
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues.
                return IdentityResult.Failed(new IdentityError
                {
                    Code = nameof(DbUpdateConcurrencyException),
                    Description = "Optimistic concurrency failure, object has been modified."
                });
            }
            catch (Exception ex)
            {
                // Handle other potential errors.
                return IdentityResult.Failed(new IdentityError
                {
                    Code = nameof(Exception),
                    Description = ex.Message
                });
            }

            // If no exceptions were thrown, return success.
            return IdentityResult.Success;
        }

        public Task SetEmailAsync(User user, string? email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Return the user's email.
            return Task.FromResult(user.Email);
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
                    .SingleOrDefaultAsync(u => u.Email.Equals(normalizedEmail), cancellationToken);
                
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while finding the user by email.", ex);
            }
        }

        public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email)!;
        }

        public Task SetNormalizedEmailAsync(User user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AuthenticatorKey = key;

            return Task.CompletedTask;
        }

        public Task<string?> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AuthenticatorKey);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            // Ensure the cancellation token is not cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the user object is not null.
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}
