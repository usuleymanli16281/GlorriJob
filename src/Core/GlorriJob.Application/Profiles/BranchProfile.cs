using AutoMapper;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Application.Dtos.Category;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class BranchProfile : Profile
	{
        public BranchProfile()
        {
			CreateMap<Branch, BranchGetDto>().ReverseMap();
		}
    }
}
