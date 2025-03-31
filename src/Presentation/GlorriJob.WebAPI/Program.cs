using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Common.Shared;
using GlorriJob.Domain.Entities;
using GlorriJob.Infrastructure;
using GlorriJob.Infrastructure.Consumers;
using GlorriJob.Persistence;
using GlorriJob.Persistence.Implementations.Repositories;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistentServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMassTransit(config =>
{
	var host = builder.Configuration["MassTransit:Host"];
	var username = builder.Configuration["MassTransit:Username"];
	var password = builder.Configuration["MassTransit:Password"];
	var port = int.Parse(builder.Configuration["MassTransit:Port"]!);
	var rabbitMqUri = new Uri($"rabbitmq://{host}:{port}");
	config.AddConsumer<SendEmailConsumer>();
	config.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host(rabbitMqUri, h =>
		{
			h.Username(builder.Configuration["MassTransit:Username"]!);
			h.Password(builder.Configuration["MassTransit:Password"]!);
		});

		cfg.ConfigureEndpoints(context);
	});

	
});
builder.Services.AddHttpContextAccessor();
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
UserContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseExceptionHandler(options =>
{
	options.Run(async context =>
	{
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		context.Response.ContentType = MediaTypeNames.Application.Json;

		var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
		if(contextFeature is not null)
		{
			await context.Response.WriteAsync(new ErrorDetail
			{
				StatusCode = context.Response.StatusCode,
				Message = "Internal Server Error",
				Detail = contextFeature.Error.Message
			}.ToString());
		}
	});
});
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();
app.Run();
