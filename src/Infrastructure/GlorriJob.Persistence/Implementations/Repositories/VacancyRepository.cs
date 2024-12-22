using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;

namespace GlorriJob.Persistence.Implementations.Repositories;

public class VacancyRepository : Repository<Vacancy>, IVacancyRepository
{
    public VacancyRepository(GlorriJobDbContext context) : base(context)
    {
    }
}

