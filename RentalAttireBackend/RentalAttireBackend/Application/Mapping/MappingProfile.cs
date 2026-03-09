using AutoMapper;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Clothes.Commands.CreateClothe;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.Commands.CreateEmployee;
using RentalAttireBackend.Application.Employees.Commands.UpdateEmployee;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Application.Persons.Commands.UpdatePerson;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Entities;
using System.CodeDom;
using System.Runtime.Serialization;

namespace RentalAttireBackend.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region Person->PersonDTO
            CreateMap<Person, PersonDTO>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => src.MaritalStatus.ToString()));
            #endregion

            #region PersonDTO->Person
            CreateMap<PersonDTO, Person>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => Enum.Parse<MaritalStatus>(src.MaritalStatus, true)))
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Person"));
            #endregion

            #region UpdatePersonCommand->Person
            CreateMap<UpdatePersonCommand, Person>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => Enum.Parse<MaritalStatus>(src.MaritalStatus, true)));
            #endregion

            #region Employee->EmployeeDTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.RolePosition,
                opt => opt.MapFrom(src => src.Role.RolePosition))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.User.Person));

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            #endregion

            #region Disposables
            CreateMap<User, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy));

            CreateMap<Employee, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy));

            CreateMap<Person, ArchivedEntityDto>()
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => src.EntityType))
                .ForMember(dest => dest.EntityId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(src => src.ArchivedAt))
                .ForMember(dest => dest.ArchivedBy,
                opt => opt.MapFrom(src => src.ArchivedBy));

            #endregion

            #region CreateEmployeeCommand->Employee
            CreateMap<CreateEmployeeCommand, Employee>()
                .ForMember(dest => dest.RoleId,
                opt => opt.MapFrom(src => Enum.Parse<RolePosition>(src.RolePosition, true)))
                .ForMember(dest => dest.Id,
                opt => opt.Ignore())
                .ForPath(dest => dest.User.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForPath(dest => dest.User.HashedPassword,
                opt => opt.Ignore());
            #endregion

            #region CreateEmployeeCommand-User
            CreateMap<CreateEmployeeCommand, User>()
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.HashedPassword,
                opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken,
                opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime,
                opt => opt.MapFrom(src => DateTime.UtcNow.AddDays(7)))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.Person))
                .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Employee"));
            #endregion

            #region UpdateEmployeeCommand -> Employee
            CreateMap<UpdateEmployeeCommand, Employee>()
                .ForMember(dest => dest.RoleId,
                opt => opt.MapFrom(src => Enum.Parse<RolePosition>(src.RolePosition, true)))
                .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.PerformedBy))
                .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(8)))
                .ForPath(dest => dest.User.Person.UpdatedBy,
                opt => opt.MapFrom(src => src.PerformedBy))
                .ForPath(dest => dest.User.Person.UpdatedAt,
                opt => opt.MapFrom(src => DateTime.UtcNow));
            #endregion

            #region User->UserDTO
            CreateMap<User, UserDTO>();
            #endregion

            #region UserViewModel
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.EmployeeCode,
                opt => opt.MapFrom(src => src.Employee.EmployeeCode))
                .ForMember(dest => dest.RolePosition,
                opt => opt.MapFrom(src => src.Employee.Role.RolePosition.ToString()))
                .ForMember(dest => dest.Department,
                opt => opt.MapFrom(src => src.Employee.Department))
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Person.FullName));
            #endregion

            #region AuditLog -> AuditLogDTO
            CreateMap<AuditLog, AuditLogDTO>();
            #endregion

            #region Employee -> Employee
            CreateMap<Employee, Employee>();
            #endregion

            #region Clothe -> ClotheDTO
            CreateMap<Clothe, ClotheDTO>()
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ClotheGender,
                opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Condition,
                opt => opt.MapFrom(src => src.Condition.ToString()))
                .ForMember(dest => dest.ProfileImagePath,
                opt => opt.MapFrom(src => src.ProfileImagePath));
            #endregion

            #region CreateClotheCommand -> Clothe
            CreateMap<CreateClotheCommand, Clothe>()
                .ForMember(c => c.CategoryId,
                opt => opt.Ignore())
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<ClotheGender>(src.ClotheGender, true)))
                .ForMember(dest => dest.Condition,
                opt => opt.MapFrom(src => Enum.Parse<Condition>(src.Condition, true)))
                .ForMember(dest => dest.ProfileImagePath,
                opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable,
                opt => opt.Ignore())
                .ForMember(dest => dest.EntityType,
                opt => opt.MapFrom(src => "Clothe"))
                .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.PerformedBy));
            #endregion

            #region Category -> CategoryDTO 
            CreateMap<Category, CategoryDTO>();
            #endregion
        }
    }
}
