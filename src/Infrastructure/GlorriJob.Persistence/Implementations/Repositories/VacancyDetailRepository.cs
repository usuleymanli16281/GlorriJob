using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;

namespace GlorriJob.Persistence.Implementations.Repositories;

public class VacancyDetailRepository : Repository<VacancyDetail>, IVacancyDetailRepository
{
    public VacancyDetailRepository(GlorriJobDbContext context) : base(context)
    {
    }
}
