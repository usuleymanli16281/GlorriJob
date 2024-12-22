using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlorriJob.Application.Dtos.Branch;
using GlorriJob.Domain.Shared;

namespace GlorriJob.Application.Abstractions.Services;

public interface IBranchService
{
    Task<BranchGetDto> GetByIdAsync(Guid id);
    Task<Pagination<BranchGetDto>> GetAllAsync(
            BranchFilterDto filterDto,
            int pageNumber = 1,
            int pageSize = 10
        );
    Task<Pagination<BranchGetDto>> SearchByNameAsync(
            string name,
            int pageNumber = 1,
            int pageSize = 10
        );
    Task<BranchGetDto> CreateAsync(BranchCreateDto createBranchDto);
    Task<BranchGetDto> UpdateAsync(Guid id, BranchUpdateDto branchUpdateDto);
    Task DeleteAsync(Guid id);
}
