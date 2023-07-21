using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.Data.Interfaces;

public interface IArtistService
{
    Task<ArtistDto> GetClientArtist(long artistId);
    List<ArtistDto> GetClientArtists();
    Task<List<Album>> GetAlbumsForArtist(long artistId);
}
