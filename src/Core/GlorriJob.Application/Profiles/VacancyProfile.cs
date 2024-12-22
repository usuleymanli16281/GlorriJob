using AutoMapper;
using GlorriJob.Application.Dtos;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Profiles;

public class VacancyProfile : Profile
{
    public VacancyProfile()
    {
        CreateMap<Vacancy, VacancyGetDto>().ReverseMap();
        CreateMap<Vacancy, VacancyCreateDto>().ReverseMap();
        CreateMap<Vacancy, VacancyUpdateDto>().ReverseMap();
        CreateMap<Vacancy, VacancyFilterDto>().ReverseMap();
    }
}
