using Chinook.ClientModels;
using Chinook.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chinook.Services.Data.Interfaces
{
    public interface IArtistService
    {
        Task<Artist> GetArtist(long artistId);
        Task<List<Artist>> GetArtists();
        Task<List<Album>> GetAlbumsForArtist(long artistId);
    }
}