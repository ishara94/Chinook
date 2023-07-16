using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Chinook.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Data
{
    public class PlayListService : IPlayListService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        

        public PlayListService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
         

        }

        public async Task<List<Chinook.Models.Playlist>> GetExistingPlaylists(string CurrentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var existingPlaylists = await dbContext.UserPlaylists
                    .Where(up => up.UserId == CurrentUserId)
                    .Select(up => up.Playlist)
                    .ToListAsync();

           return existingPlaylists;
            
        }

        public async Task<Chinook.ClientModels.Playlist> GetPlayLists(long PlaylistId,string CurrentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var playLists = dbContext.Playlists
             .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
             .Where(p => p.PlaylistId == PlaylistId)
             .Select(p => new ClientModels.Playlist()
             {
                 Name = p.Name,
                 Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                 {
                     AlbumTitle = t.Album.Title,
                     ArtistName = t.Album.Artist.Name,
                     TrackId = t.TrackId,
                     TrackName = t.Name,
                     IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites")).Any()
                 }).ToList()
             })
             .FirstOrDefault();

            return playLists;
        }

        public async Task<Chinook.Models.Playlist> CreateNewPlaylist(string CurrentUserId, string playlistName)
        {
            using var context = _dbFactory.CreateDbContext();
            var maxPlaylistId = context.Playlists.Max(p => p.PlaylistId);

            var playlist = new Chinook.Models.Playlist
            {
                Name = playlistName,
                PlaylistId = maxPlaylistId + 1
            };

            var userPlaylist = new UserPlaylist
            {
                UserId = CurrentUserId,
                Playlist = playlist
            };

            context.UserPlaylists.Add(userPlaylist);
            await context.SaveChangesAsync();         
            return playlist;
        }

        public async Task<bool> AddTrackToPlaylist(long playlistId, long trackId)
        {
            using var context = _dbFactory.CreateDbContext();
            var playlist = await context.Playlists.FindAsync(playlistId);
            var track = await context.Tracks.FindAsync(trackId);

            if (playlist == null || track == null)
            {
                return false;
            }

            playlist.Tracks.Add(track);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
