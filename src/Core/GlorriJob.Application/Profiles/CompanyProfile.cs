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
			CreateMap<Company, CompanyGetDto>().ReverseMap();
			CreateMap<Company, CompanyCreateDto>().ReverseMap();
			CreateMap<Company, CompanyUpdateDto>().ReverseMap();
		}
    }
}
