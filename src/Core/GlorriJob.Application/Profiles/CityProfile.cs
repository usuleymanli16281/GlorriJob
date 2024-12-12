using AutoMapper;
using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, GetCityDto>().ReverseMap();
    }
}
