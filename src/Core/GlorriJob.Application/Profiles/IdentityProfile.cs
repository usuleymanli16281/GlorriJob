using AutoMapper;
using GlorriJob.Application.Dtos.Identity;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class IdentityProfile : Profile
	{
        public IdentityProfile()
        {
            CreateMap<User, RegisterDto>().ReverseMap();
        }
    }
}
