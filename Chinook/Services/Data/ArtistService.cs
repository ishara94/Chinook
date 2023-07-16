using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using System.Security.Claims;

namespace Chinook.Services.Data
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;      
        public ArtistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;           
        }

        public async Task<Artist> GetArtist(long artistId)
        {  
            using var  dbContext= _dbFactory.CreateDbContext();
            var artist = await dbContext.Artists.SingleOrDefaultAsync(a => a.ArtistId == artistId);           
            return artist;
        }
        public async Task<List<Artist>> GetArtists()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var updatedArtistList = new List<Artist>();
           // var users = dbContext.Users.Include(a => a.UserPlaylists).ToList();
            
            // To fix the Bug in NUmberofAlbums in Index page
            foreach (var artist in dbContext.Artists)
            {
                artist.Albums =await  GetAlbumsForArtist(artist.ArtistId);
                updatedArtistList.Add(artist);
            }
            return updatedArtistList;
        }

        public async Task<List<Album>> GetAlbumsForArtist(long artistId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return dbContext.Albums.Where(a => a.ArtistId == artistId).ToList();
        }
    }
}
