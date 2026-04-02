using Microsoft.EntityFrameworkCore;
using ApiAlumnos2026.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApiAlumnos2026DbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("ApiAlumnos2026DbContext")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(
        policy =>
        {
            // policy.AnyOrigins();

            policy.WithOrigins();

            policy.AllowAnyMethod();

            policy.AllowAnyHeader();

        });
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
