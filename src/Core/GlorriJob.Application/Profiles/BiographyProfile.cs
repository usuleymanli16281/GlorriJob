using AutoMapper;
using GlorriJob.Application.Dtos.Biography;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class BiographyProfile : Profile
	{
        public BiographyProfile()
        {
            CreateMap<Biography,BiographyGetDto>().ReverseMap();
            CreateMap<Biography,BiographyCreateDto>().ReverseMap();
            CreateMap<Biography,BiographyUpdateDto>().ReverseMap();
        }
    }
}
