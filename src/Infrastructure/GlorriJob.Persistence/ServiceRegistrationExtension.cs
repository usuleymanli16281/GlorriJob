using FluentValidation;
using GlorriJob.Application.Abstractions.Repositories;
using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Application.Profiles;
using GlorriJob.Application.Validations.City;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence.Contexts;
using GlorriJob.Persistence.Implementations.Repositories;
using GlorriJob.Persistence.Implementations.Services;
using Hangfire;
using Hangfire.PostgreSql;
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
        services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.User.RequireUniqueEmail = true;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<GlorriJobDbContext>();


		services.AddHangfire(config =>
		{
			config.UsePostgreSqlStorage(options =>
			{
				options.UseNpgsqlConnection(configuration.GetConnectionString("Default"));
			},
			new PostgreSqlStorageOptions
			{
				SchemaName = "hangfire"
			});
		});
		services.AddHangfireServer();

		services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICityService, CityService>();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IIndustryRepository, IndustryRepository>();
        services.AddScoped<IIndustryService, IndustryService>();

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyService, CompanyService>();

        services.AddScoped<IBiographyRepository, BiographyRepository>();
        services.AddScoped<IBiographyService, BiographyService>();

        services.AddScoped<IVacancyRepository, VacancyRepository>();
        services.AddScoped<IVacancyService, VacancyService>();

        services.AddScoped<IVacancyDetailRepository, VacancyDetailRepository>();
        services.AddScoped<IVacancyDetailService, VacancyDetailService>();

        services.AddScoped<ICompanyDetailRepository, CompanyDetailRepository>();
        services.AddScoped<ICompanyDetailService, CompanyDetailService>();

        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IBranchService, BranchService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddAutoMapper(typeof(CityProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<CityUpdateValidator>();

        return services;
    }
}
