using AutoMapper;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Application.Persons.Commands.UpdatePerson;
using RentalAttireBackend.Application.Persons.DTO;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Person, PersonDTO>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => src.MaritalStatus.ToString()));

            CreateMap<UpdatePersonCommand, Person>()
                .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => Enum.Parse<Gender>(src.Gender, true)))
                .ForMember(dest => dest.MaritalStatus,
                opt => opt.MapFrom(src => Enum.Parse<MaritalStatus>(src.MaritalStatus, true)));

            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.RolePosition,
                opt => opt.MapFrom(src => src.Role.RolePosition))
                .ForMember(dest => dest.Person,
                opt => opt.MapFrom(src => src.Person));

        }
    }
}
