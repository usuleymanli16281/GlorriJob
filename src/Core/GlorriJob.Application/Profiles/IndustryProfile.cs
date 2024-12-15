using AutoMapper;
using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Profiles;

public class IndustryProfile : Profile
{
    public IndustryProfile()
    {
        CreateMap<Industry, IndustryGetDto>().ReverseMap();
        CreateMap<Industry, IndustryCreateDto>().ReverseMap();
        CreateMap<Industry, IndustryUpdateDto>().ReverseMap();
    }
}
