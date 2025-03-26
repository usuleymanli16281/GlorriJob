using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Domain.Entities;
using GlorriJob.Infrastructure;
using GlorriJob.Persistence;
using GlorriJob.Persistence.Implementations.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistentServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter your JWT token in the format: Bearer {your_token}"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseHangfireDashboard("/hangfire");
}
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var userManager = services.GetRequiredService<UserManager<User>>();
		var roleManager = services.GetRequiredService<RoleManager<Role>>();
		await DbSeeder.SeedRolesAndUsers(userManager, roleManager);
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error seeding roles and users: {ex.Message}");
	}
}
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();
app.Run();
