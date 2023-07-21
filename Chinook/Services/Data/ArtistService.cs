using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services.Data;

public class ArtistService : IArtistService
{
    private readonly IDbContextFactory<ChinookContext> _dbFactory;
    private readonly IMapper _mapper;

    public ArtistService(IDbContextFactory<ChinookContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<ArtistDto> GetClientArtist(long artistId)
    {
        try
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var artist = await dbContext.Artists.Include(p => p.Albums).SingleOrDefaultAsync(a => a.ArtistId == artistId);
            if (artist == null) return null;
            // Data Mapping using automapper
            var clientArtist = _mapper.Map<ArtistDto>(artist);
            return clientArtist;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving the artist.", ex);
        }
    }

    public async Task<List<ArtistDto>> GetClientArtists()
    {
        try
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var updatedArtistList = new List<ArtistDto>();

            foreach (var artist in dbContext.Artists.Include(a => a.Albums))
            {
                var clientArtist = _mapper.Map<ArtistDto>(artist);
                updatedArtistList.Add(clientArtist);
            }

            return updatedArtistList;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving the artists.", ex);
        }
    }

    // Keep the existing GetAlbumsForArtist method as it already returns DB model
    public async Task<List<Album>> GetAlbumsForArtist(long artistId)
    {
        try
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.Albums.Where(a => a.ArtistId == artistId).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving the albums for the artist.", ex);
        }
    }
}

