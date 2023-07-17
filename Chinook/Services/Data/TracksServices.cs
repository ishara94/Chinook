using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Microsoft.Data.Sqlite;
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

        // Retrieves the tracks
        public async Task<List<PlaylistTrack>> GetTracks(Artist artist, string currentUserId)
        {
            try
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
            catch (Exception ex)
            {         
                throw new Exception("Error occurred while retrieving tracks.", ex);
            }
        }

        // Updates the favorite track status
        private async Task<Chinook.Models.Playlist> CreateFavouitePlayList(string currentUserId)
        {
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();
                var maxPlaylistId = dbContext.Playlists.Max(p => p.PlaylistId);               
                
                var favoritesPlaylist = new Chinook.Models.Playlist { Name = "Favorites", PlaylistId = maxPlaylistId+ 1};
                dbContext.Playlists.Add(favoritesPlaylist);
                await dbContext.SaveChangesAsync();              

               favoritesPlaylist.UserPlaylists ??= new List<UserPlaylist>();

                var userplaylist = new UserPlaylist
                {
                    UserId = currentUserId,
                    Playlist = favoritesPlaylist
                };

                dbContext.UserPlaylists.Add(userplaylist);
                await dbContext.SaveChangesAsync();

                var favoritesdata = await dbContext.Playlists                    
                    .Include(c => c.Tracks)
                    .FirstOrDefaultAsync(p => p.Name == "Favorites" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

                return favoritesdata;
            }
            catch (Exception ex)
            {              
                throw new Exception("Error occurred while updating the favorites playlist.", ex);
            }
        }

        public async Task UpdateFavoriteTrackStatus(long trackId, string currentUserId, bool isFavorite)
        {
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();

                var track = await dbContext.Tracks
                    .Include(t => t.Playlists)
                    .FirstOrDefaultAsync(t => t.TrackId == trackId);

                var favoritesPlaylist = await dbContext.Playlists
                    .FirstOrDefaultAsync(p => p.Name == "Favorites" && p.UserPlaylists.Any(up => up.UserId == currentUserId));

                // FirstTime
                if (favoritesPlaylist == null && isFavorite)
                {
                    favoritesPlaylist = await CreateFavouitePlayList(currentUserId);
                }

                if (isFavorite)
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

                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) {
                await UpdateFavoriteTrackStatus(trackId, currentUserId, isFavorite);}
            catch(Exception ex) { throw new Exception("Error occurred while updating the favorite track status.", ex); }   
        }

        public async Task  RemoveTrackFromPlayList(long trackId, long playListId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var playList = dbContext.Playlists.Include(c=>c.Tracks)
                .FirstOrDefault(p => p.PlaylistId == playListId);
            var trackToRemove = playList.Tracks.FirstOrDefault(t => t.TrackId == trackId);

            if (playList != null)
            {           
                var track = playList.Tracks.Remove(trackToRemove);
               await dbContext.SaveChangesAsync();
            }
        }
    }
}