using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Domain.Entities;
using GlorriJob.Persistence;
using GlorriJob.Persistence.Implementations.Repositories;
using GlorriJob.Persistence.Implementations.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistentServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
