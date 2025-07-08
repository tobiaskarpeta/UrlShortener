using AutoMapper;
using UrlShortener.Api.Features.Urls.Commands;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Mappings;

public class ShortenedUrlProfile : Profile
{
    public ShortenedUrlProfile()
    {
        CreateMap<UpdateShortenedUrlCommand, ShortenedUrl>()
            .ForMember(x => x.UniqueId, opt => opt.Ignore());
    }
}