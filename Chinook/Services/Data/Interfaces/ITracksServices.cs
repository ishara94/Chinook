using Chinook.ClientModels;
using Chinook.Models;
using NuGet.DependencyResolver;

namespace Chinook.Services.Data.Interfaces
{
    public interface ITracksServices
    {
        Task<List<PlaylistTrack>> GetTracks(Artist artist, string currentUserId);
        Task UpdateFavoriteTrackStatus(long trackId, string CurrentUserId, bool isfavourite);

        Task RemoveTrackFromPlayList(long trackId,long playListId);
    }
}