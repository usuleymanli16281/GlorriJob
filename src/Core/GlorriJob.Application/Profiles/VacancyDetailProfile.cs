using AutoMapper;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Domain.Entities;

namespace GlorriJob.Application.Profiles;

public class VacancyDetailProfile : Profile
{
    public VacancyDetailProfile()
    {
        CreateMap<VacancyDetail, VacancyDetailGetDto>().ReverseMap();
        CreateMap<VacancyDetail, VacancyDetailCreateDto>().ReverseMap();
        CreateMap<VacancyDetail, VacancyDetailUpdateDto>().ReverseMap();
    }
}
