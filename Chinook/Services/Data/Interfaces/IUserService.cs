using Microsoft.AspNetCore.Components.Authorization;

namespace Chinook.Services.Data.Interfaces;

public interface IUserService
{
    Task<string> GetUserId(Task<AuthenticationState> authenticationState);
}
