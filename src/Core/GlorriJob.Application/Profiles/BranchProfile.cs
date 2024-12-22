using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GlorriJob.Application.Dtos.Vacancy;
using GlorriJob.Application.Dtos;
using GlorriJob.Domain.Entities;
using GlorriJob.Application.Dtos.Branch;

namespace GlorriJob.Application.Profiles;

internal class BranchProfile : Profile
{
    public BranchProfile()
    {
        CreateMap<Branch, BranchGetDto>().ReverseMap();
        CreateMap<Branch, BranchCreateDto>().ReverseMap();
        CreateMap<Branch, BranchUpdateDto>().ReverseMap();
        CreateMap<Branch, BranchFilterDto>().ReverseMap();
    }
}
