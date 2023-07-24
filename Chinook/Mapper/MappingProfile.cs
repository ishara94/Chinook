using AutoMapper;
using Chinook.ClientModels;
using Chinook.Models;
namespace Chinook.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Artist, Artist>()
            .ForMember(dest => dest.NumberOfAlbums, opt => opt.MapFrom(src => src.Albums.Count));

        CreateMap<Models.Playlist, ClientModels.Playlist>();

        CreateMap<Track, ClientModels.PlaylistTrack>()
            .ForMember(dest => dest.TrackName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.AlbumTitle, opt => opt.MapFrom(src => src.Album.Title))
            .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Album.Artist.Name));

    }
}

