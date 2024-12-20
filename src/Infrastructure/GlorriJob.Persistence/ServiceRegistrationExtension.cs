﻿using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Profiles;
using GlorriJob.Application.Validations.City;
using GlorriJob.Persistence.Contexts;
using GlorriJob.Persistence.Implementations.Repositories;
using GlorriJob.Persistence.Implementations.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlorriJob.Persistence;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddPersistentServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GlorriJobDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<GlorriJobDbContext>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IIndustryRepository, IndustryRepository>();
        services.AddScoped<IIndustryService, IndustryService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddAutoMapper(typeof(CityProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<CityUpdateValidator>();
        return services;
    }
}
