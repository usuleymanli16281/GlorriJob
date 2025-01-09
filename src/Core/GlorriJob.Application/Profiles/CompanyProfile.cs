using AutoMapper;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Application.Dtos.Company;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class CompanyProfile : Profile
	{
        public CompanyProfile()
        {
			CreateMap<Company, CompanyGetDto>()
				.ForMember(dest => dest.DepartmentGetDtos, opt => opt.MapFrom(src => src.Departments))
				.ForMember(dest => dest.IndustryGetDto, opt => opt.MapFrom(src => src.Industry))
				.ReverseMap();
			CreateMap<Company, CompanyCreateDto>().ReverseMap();
			CreateMap<Company, CompanyUpdateDto>().ReverseMap();
		}
    }
}
