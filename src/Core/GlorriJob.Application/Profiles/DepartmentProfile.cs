using AutoMapper;
using GlorriJob.Application.Dtos.Department;
using GlorriJob.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Profiles
{
	public class DepartmentProfile : Profile
	{
        public DepartmentProfile()
        {
            CreateMap<Department,DepartmentGetDto>().ReverseMap();
            CreateMap<Department,DepartmentUpdateDto>().ReverseMap();
            CreateMap<Department,DepartmentCreateDto>().ReverseMap();
        }
    }
}
