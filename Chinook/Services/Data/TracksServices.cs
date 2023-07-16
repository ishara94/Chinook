using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Data
{
    public class TracksServices : ITracksServices
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
      
        public TracksServices(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;         
        }

        public async Task<List<PlaylistTrack>> GetTracks(Artist artist, string currentUserId)
        {

            using var dbContext = _dbFactory.CreateDbContext();
            var tracks = await dbContext.Tracks
                .Where(a => a.Album.ArtistId == artist.ArtistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists
                        .Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == "Favorites"))
                        .Any()
                })
                .ToListAsync();

            return tracks;
        }

        public async Task<bool> UpdateFavoriteTrackStatus(long trackId, string currentUserId, bool isfavourite)
        {

            using var dbContext = _dbFactory.CreateDbContext();

            var track = await dbContext.Tracks
                .Include(t => t.Playlists)
                .FirstOrDefaultAsync(t => t.TrackId == trackId);

            var favoritesPlaylist = await dbContext.Playlists
                .FirstOrDefaultAsync(p => p.Name == "Favorites" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

            if (favoritesPlaylist == null && isfavourite)
            {                
                favoritesPlaylist = new Chinook.Models.Playlist { Name = "Favorites" };
                favoritesPlaylist.UserPlaylists.Add(new UserPlaylist { UserId = currentUserId });
                dbContext.Playlists.Add(favoritesPlaylist);
            }

            if (isfavourite)
            {
                if (!track.Playlists.Contains(favoritesPlaylist))
                {
                    track.Playlists.Add(favoritesPlaylist);
                }
            }
            else
            {
                if (track.Playlists.Contains(favoritesPlaylist))
                {
                    track.Playlists.Remove(favoritesPlaylist);
                }
            }

            await dbContext.SaveChangesAsync();

            return true;       
        }   
    } 
}
