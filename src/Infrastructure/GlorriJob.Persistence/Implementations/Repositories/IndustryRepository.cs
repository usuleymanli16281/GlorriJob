using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;

namespace GlorriJob.Persistence.Implementations.Repositories;

public class IndustryRepository : Repository<Industry>, IIndustryRepository
{
    public IndustryRepository(GlorriJobDbContext context) : base(context)
    {
    }
}
