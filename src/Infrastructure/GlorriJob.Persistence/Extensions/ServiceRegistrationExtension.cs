﻿using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Profiles;
using GlorriJob.Application.Validations.City;
using GlorriJob.Application.Validations.Industry;
using GlorriJob.Persistence.Contexts;
using GlorriJob.Persistence.Implementations.Repositories;
using GlorriJob.Persistence.Implementations.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Persistence.Extensions
{
	public static class ServiceRegistrationExtension
	{
		public static IServiceCollection AddPersistentServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<GlorriJobDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Default")));
			services.AddScoped<ICityRepository, CityRepository>();
			services.AddScoped<IIndustryRepository, IndustryRepository>();
			services.AddScoped<ICityService, CityService>();
			services.AddScoped<IIndustryService, IndustryService>();
			services.AddAutoMapper(typeof(CityProfile).Assembly);
			services.AddAutoMapper(typeof(IndustryProfile).Assembly);
			services.AddValidatorsFromAssemblyContaining<CityUpdateValidator>();
			services.AddValidatorsFromAssemblyContaining<IndustryUpdateValidator>();
			return services;
		}
	}
}
