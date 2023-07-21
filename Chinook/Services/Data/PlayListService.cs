using AutoMapper;
using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Data;

public class PlayListService : IPlayListService
{
    private readonly IDbContextFactory<ChinookContext> _dbFactory;
    private readonly IMapper _mapper;

    public PlayListService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<List<ClientModels.Playlist>> GetExistingPlaylists(string CurrentUserId)
    {
        try
        {
            using var dbContext = _dbFactory.CreateDbContext();
            // Retrieve the existing playlists for the current user
            var existingPlaylists = await dbContext.UserPlaylists
                .Where(up => up.UserId == CurrentUserId)
                .Select(up => up.Playlist)
                .ToListAsync();

            // Data mapping using Automapper
            var Clientplaylist = _mapper.Map<List<Chinook.ClientModels.Playlist>>(existingPlaylists);


            // Check if the "Favourite" playlist exists in the list
            var favouritePlaylist = Clientplaylist.FirstOrDefault(p => p.Name == "Favorites");
            if (favouritePlaylist != null)
            {
                // Remove the "Favourite" playlist from its current position and insert it at the beginning
                Clientplaylist.Remove(favouritePlaylist);
                Clientplaylist.Insert(0, favouritePlaylist);
            }

            return Clientplaylist;
        }
        catch (Exception ex)
        {

            throw new Exception("Error occurred while retrieving the existing playlists.", ex);
        }
       
    }

    public async Task<ClientModels.Playlist> GetPlayLists(long PlaylistId, string CurrentUserId)
    {
        try
        {
            using var dbContext = _dbFactory.CreateDbContext();

            // Retrieve the playlist
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
                        IsFavorite = t.Playlists
                            .Where(p => p.UserPlaylists
                                .Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites"))
                            .Any()
                    }).ToList()
                })
                .FirstOrDefault();

            return playLists;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving the playlist.", ex);
        }
        
    }

    public async Task<Playlist> CreateNewPlaylist(string CurrentUserId, string playlistName)
    {
        try
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var maxPlaylistId = dbContext.Playlists.Max(p => p.PlaylistId);
            var existPlaylistName = dbContext.UserPlaylists
                .Where(u => u.UserId == CurrentUserId)
                .FirstOrDefault(p => p.Playlist.Name == playlistName);

            // Create a new playlist with the provided playlistName and the generated PlaylistId
            if (existPlaylistName == null)
            {
                var playlist = new Chinook.Models.Playlist
                {
                    Name = playlistName,
                    PlaylistId = maxPlaylistId + 1
                };

                // Create a UserPlaylist entry linking the current user to the new playlist
                var userPlaylist = new UserPlaylist
                {
                    UserId = CurrentUserId,
                    Playlist = playlist
                };

                dbContext.UserPlaylists.Add(userPlaylist);
                await dbContext.SaveChangesAsync();

                return playlist;
            }
            return dbContext.Playlists.FirstOrDefault(p => p.PlaylistId == existPlaylistName.PlaylistId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while creating a new playlist.", ex);
        }
       

    }



    public async Task<string> AddTrackToPlaylist(long playlistId, long trackId)
    {
        try
        {

            using var dbContext = _dbFactory.CreateDbContext();

            // Find the playlist and track entities
            var playlist = await dbContext.Playlists
                      .Include(p => p.Tracks)
                      .FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
            var track = await dbContext.Tracks.FindAsync(trackId);

            if (playlist == null || track == null)
            {
                // Handle the case where either the playlist or track is not found
                return null;
            }
            if (!playlist.Tracks.Any(t => t.TrackId == trackId))
            {
                playlist.Tracks.Add(track);
            }

            await dbContext.SaveChangesAsync();

            return playlist.Name;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while adding a track to the playlist.", ex);
        }


    }
}
