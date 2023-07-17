using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();
                // Retrieve the artist with using artistId
                var artist = await dbContext.Artists.SingleOrDefaultAsync(a => a.ArtistId == artistId);
                return artist;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving the artist.", ex);
            }
        }

        public async Task<List<Artist>> GetArtists()
        {
            try
            {
                // Create the DbContext using the factory
                using var dbContext = _dbFactory.CreateDbContext();
                var updatedArtistList = new List<Artist>();

                foreach (var artist in dbContext.Artists)
                {
                    // Get albums for each artist
                    artist.Albums = await GetAlbumsForArtist(artist.ArtistId);
                    updatedArtistList.Add(artist);
                }

                return updatedArtistList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving the artists.", ex);
            }
        }

        public async Task<List<Album>> GetAlbumsForArtist(long artistId)
        {
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();
                // Retrieve the albums for the using artistId
                return await dbContext.Albums.Where(a => a.ArtistId == artistId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving the albums for the artist.", ex);
            }
        }
    }
}
