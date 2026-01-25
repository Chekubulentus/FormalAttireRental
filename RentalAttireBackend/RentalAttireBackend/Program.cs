using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Application;
using RentalAttireBackend.Application.Mapping;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//AutoMapper
builder.Services.AddAutoMapper(x => x.AddProfile<MappingProfile>());

//DbContext
builder.Services.AddDbContext<FormalAttireContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("FormalAttireDb")));

//MediatR
builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
