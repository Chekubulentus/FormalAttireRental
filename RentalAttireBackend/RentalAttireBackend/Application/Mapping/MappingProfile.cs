using AutoMapper;
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
        }
    }
}
