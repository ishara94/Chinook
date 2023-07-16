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

        private  async Task<Chinook.Models.Playlist> DBUpdate(string currentUserId)
        {           
                using var dbContext = _dbFactory.CreateDbContext();
                var favoritesPlaylist = dbContext.Playlists.FirstOrDefault(p => p.Name == "Favorites");
                               
                if (favoritesPlaylist == null)
                {
                    favoritesPlaylist = new Chinook.Models.Playlist { Name = "Favorites",PlaylistId=0 };
                    dbContext.Playlists.Add(favoritesPlaylist);
                }

                favoritesPlaylist.UserPlaylists ??= new List<UserPlaylist>();

                var userplaylist = new UserPlaylist
                {
                    UserId = currentUserId,
                    Playlist = favoritesPlaylist
                };

                dbContext.UserPlaylists.Add(userplaylist);
                await dbContext.SaveChangesAsync();
                
               var favoritesdata = await dbContext.Playlists
                .FirstOrDefaultAsync(p => p.Name == "Favorites" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

            return favoritesdata;
        }
        public async Task UpdateFavoriteTrackStatus(long trackId, string currentUserId, bool isfavourite)
        {

            using var dbContext = _dbFactory.CreateDbContext();

            var track = await dbContext.Tracks
                .Include(t => t.Playlists)
                .FirstOrDefaultAsync(t => t.TrackId == trackId);

            var favoritesPlaylist = await dbContext.Playlists
                .FirstOrDefaultAsync(p => p.Name == "Favorites" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

            // FirstTime
            if (favoritesPlaylist == null && isfavourite)
            {
              favoritesPlaylist= await DBUpdate(currentUserId);
               
            }
       
            if (isfavourite)
            {
                if (!track.Playlists.Contains(favoritesPlaylist))
                {
                    if (!dbContext.Entry(favoritesPlaylist).IsKeySet)
                    {
                        dbContext.Playlists.Attach(favoritesPlaylist);
                    }

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

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                await UpdateFavoriteTrackStatus(trackId, currentUserId, isfavourite);
            }                            
        }   
    } 
}
