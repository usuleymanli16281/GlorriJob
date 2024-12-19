using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Repositories
{
	public class DepartmentRepository : Repository<Department>, IDepartmentRepository
	{
        public DepartmentRepository(GlorriJobDbContext context) : base(context)
        {
            
        }
    }
}
