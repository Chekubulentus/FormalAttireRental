using AutoMapper;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.Commands.CreateEmployee;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Application.Persons.Commands.UpdatePerson;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Entities;
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
                .ForMember(dest => dest.Id,
                opt => opt.Ignore());
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
                opt => opt.MapFrom(src => src.Person));

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            #endregion

            #region CreateEmployeeCommand->Employee
            CreateMap<CreateEmployeeCommand, Employee>()
                .ForMember(dest => dest.RoleId,
                opt => opt.MapFrom(src => Enum.Parse<RolePosition>(src.RolePosition, true)))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.Person))
                .ForMember(dest => dest.Id,
                opt => opt.Ignore())
                .ForMember(dest => dest.PersonId,
                opt => opt.Ignore());
            #endregion

            #region User->UserDTO
            CreateMap<User, UserDTO>();
            #endregion
        }
    }
}
