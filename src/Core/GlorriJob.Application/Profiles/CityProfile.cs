using AutoMapper;
using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityGetDto>().ReverseMap();
        CreateMap<City, CityCreateDto>().ReverseMap();
        CreateMap<City, CityUpdateDto>().ReverseMap();
    }
}
