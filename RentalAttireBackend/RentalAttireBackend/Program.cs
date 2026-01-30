using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalAttireBackend.Application;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Mapping;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Infrastructure.Authentication;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using RentalAttireBackend.Infrastructure.Persistence.Repositories;
using RentalAttireBackend.Infrastructure.Persistence.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Jwt Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero,
        };
    });


//AutoMapper
builder.Services.AddAutoMapper(x => x.AddProfile<MappingProfile>());

//DbContext
builder.Services.AddDbContext<FormalAttireContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("FormalAttireDb")));

//MediatR
builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

//Add Scopes
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

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
