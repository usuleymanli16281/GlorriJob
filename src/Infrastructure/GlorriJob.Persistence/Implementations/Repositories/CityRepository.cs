using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities;
using GlorriJob.Domain.Entities.Common;
using GlorriJob.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Implementations.Repositories
{
	public class CityRepository : Repository<City>, ICityRepository
	{
        public CityRepository(GlorriJobDbContext context) : base(context)
        {
            
        }
    }
}
