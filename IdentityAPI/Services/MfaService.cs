using IdentityAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityAPI.Services;

public class MfaService : IMfaService
{
    private readonly UserManager<User> _userManager;

    public MfaService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> SetMfaMethodAsync(User user, MfaMethod method)
    {
        user.MfaMethod = method;
        if (method == MfaMethod.Authenticator && string.IsNullOrEmpty(user.AuthenticatorKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DisableMfaAsync(User user)
    {
        user.MfaMethod = MfaMethod.None;
        await _userManager.SetTwoFactorEnabledAsync(user, false);
        return await _userManager.UpdateAsync(user);
    }

    public Task<MfaMethod> GetMfaMethodAsync(User user)
    {
        return Task.FromResult(user.MfaMethod);
    }
}