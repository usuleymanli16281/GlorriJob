using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.Branch;

public record BranchFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string SortBy { get; set; } = "Name"; 
    public string SortOrder { get; set; } = "asc"; 

    public bool? IsMain { get; set; }
    public Guid? CityId { get; set; }
    public Guid? CompanyId { get; set; }
    public IEnumerable<Guid>? DepartmentIds { get; set; }
    public IEnumerable<Guid>? VacancyIds { get; set; }


}
