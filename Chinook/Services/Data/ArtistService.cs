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
        using var dbContext = _dbFactory.CreateDbContext();
        var artist = await dbContext.Artists.Include(p => p.Albums).SingleOrDefaultAsync(a => a.ArtistId == artistId);
        if (artist == null) return null;
        // Data Mapping using automapper
        try
        {
            var clientArtist = _mapper.Map<ArtistDto>(artist);
            return clientArtist;

        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while mapping Artist entity to ArtistDto.", ex);
        }
    }

    public List<ArtistDto> GetClientArtists()
    {
        using var dbContext = _dbFactory.CreateDbContext();
        var updatedArtistList = new List<ArtistDto>();

        try
        {
            foreach (var artist in dbContext.Artists.Include(a => a.Albums))
            {
                var clientArtist = _mapper.Map<ArtistDto>(artist);
                updatedArtistList.Add(clientArtist);
            }

            return updatedArtistList;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while retrieving artists.", ex);
        }
    }

    // Keep the existing GetAlbumsForArtist method as it already returns DB model
    public async Task<List<Album>> GetAlbumsForArtist(long artistId)
    {
        using var dbContext = _dbFactory.CreateDbContext();
        return await dbContext.Albums.Where(a => a.ArtistId == artistId).ToListAsync();
    }
}

