using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Persistence.Contexts;
using GlorriJob.Persistence.Implementations.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Extensions
{
	public static class ServiceRegistrationExtension
	{
		public static IServiceCollection AddPersistentServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<GlorriJobDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Default")));
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
			return services;
		}
	}
}
