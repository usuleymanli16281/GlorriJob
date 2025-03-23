using AutoMapper;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class VacancyDetailProfile : Profile
	{
        public VacancyDetailProfile()
        {
			CreateMap<VacancyDetail, VacancyDetailGetDto>().ReverseMap();
			CreateMap<VacancyDetail, VacancyDetailCreateDto>().ReverseMap();
			CreateMap<VacancyDetail, VacancyDetailUpdateDto>().ReverseMap();
		}
    }
}
