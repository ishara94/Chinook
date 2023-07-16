using Chinook.Services.Data.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Services.Data
{
    public class UserService : IUserService
    {
        public async Task<string> GetUserId(Task<AuthenticationState> authenticationState)
        {
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }
    }
}
