using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Infrastructure.Consumers;
using GlorriJob.Infrastructure.Services;
using GlorriJob.Persistence.Implementations.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;

namespace GlorriJob.Infrastructure;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization(options =>
        {
			options.AddPolicy("UserPolicy", p => p.RequireRole("user","admin"));
			options.AddPolicy("AdminPolicy", p => p.RequireRole("admin"));
		});


		services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IImageKitService, ImageKitService>();
        services.AddScoped<IOtpCacheService, OtpCacheService>();
        services.AddScoped<IEmailService,  EmailService>();
		services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration["RedisSettings:Path"]!));
		return services;
    }
};
