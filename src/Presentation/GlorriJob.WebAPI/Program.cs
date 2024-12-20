using GlorriJob.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var app = builder.Build();

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddPersistentServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
