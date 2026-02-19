using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Application.Persons.DTO;

namespace RentalAttireBackend.Application.Users.DTO
{
    public class UserWithRelationships : UserDTO
    {
        public EmployeeDTO Employee { get; set; } = new();
        public PersonDTO Person { get; set; } = new();
    }
}
