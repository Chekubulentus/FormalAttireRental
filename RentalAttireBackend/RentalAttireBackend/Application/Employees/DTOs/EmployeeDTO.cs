using RentalAttireBackend.Application.Persons.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalAttireBackend.Application.Employees.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public double Salary { get; set; }
        public string RolePosition { get; set; } = string.Empty;
        public PersonDTO Person { get; set; } = null!;
    }
}
