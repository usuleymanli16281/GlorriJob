using AutoMapper;
using GlorriJob.Application.Dtos.CompanyDetail;
using GlorriJob.Application.Dtos.VacancyDetail;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class CompanyDetailProfile : Profile
	{
		public CompanyDetailProfile()
		{
			CreateMap<CompanyDetail, CompanyDetailGetDto>().ReverseMap();
			CreateMap<CompanyDetail, CompanyDetailCreateDto>().ReverseMap();
			CreateMap<CompanyDetail, CompanyDetailUpdateDto>().ReverseMap();
		}
	}
}
