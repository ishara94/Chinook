using Chinook.Services.Data.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Services.Data;

public class UserService : IUserService
{
    public async Task<string> GetUserId(Task<AuthenticationState> authenticationState)
    {
        try
        {
            // Retrieve the authentication state
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;

            return userId;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving the user's identifier.", ex);
        }
    }
}

