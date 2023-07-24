using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.Data.Interfaces;

public interface IArtistService
{
    Task<ClientModels.Artist> GetClientArtist(long artistId);
    List<ClientModels.Artist> GetClientArtists();
    Task<List<Album>> GetAlbumsForArtist(long artistId);
}
