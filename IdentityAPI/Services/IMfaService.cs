using IdentityAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityAPI.Services;

public interface IMfaService
{
    Task<IdentityResult> SetMfaMethodAsync(User user, MfaMethod method);
    Task<IdentityResult> DisableMfaAsync(User user);
    Task<MfaMethod> GetMfaMethodAsync(User user);
}