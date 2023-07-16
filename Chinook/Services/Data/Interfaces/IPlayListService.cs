using Chinook.ClientModels;

namespace Chinook.Services.Data.Interfaces
{
    public interface IPlayListService
    {
        Task<Playlist> GetPlayLists(long PlaylistId,string CurrentUserId);

        Task<List<Chinook.Models.Playlist>> GetExistingPlaylists(string CurrentUserId);

        Task<Chinook.Models.Playlist> CreateNewPlaylist(string CurrentUserId, string playlistName);
        Task<bool> AddTrackToPlaylist(long playlistId, long trackId);
    }
}