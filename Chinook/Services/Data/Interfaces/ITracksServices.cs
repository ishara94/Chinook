using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Services.Data.Interfaces
{
    public interface ITracksServices
    {
        Task<List<PlaylistTrack>> GetTracks(Artist artist, string currentUserId);
        Task<bool> UpdateFavoriteTrackStatus(long trackId, string CurrentUserId, bool isfavourite);
    }
}