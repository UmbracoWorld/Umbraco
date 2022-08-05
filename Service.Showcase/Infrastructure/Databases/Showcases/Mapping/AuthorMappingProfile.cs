namespace Service.Showcase.Infrastructure.Databases.Showcases.Mapping;

using AutoMapper;
using Infrastructure = Models;

internal class AuthorMappingProfile : Profile
{
    public AuthorMappingProfile()
    {
        _ = this.CreateMap<Infrastructure.Showcase, Application.Showcase.Entities.Showcase>()
            .ReverseMap();
    }
}