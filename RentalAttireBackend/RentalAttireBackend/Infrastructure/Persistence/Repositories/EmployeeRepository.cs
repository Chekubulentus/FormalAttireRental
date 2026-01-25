using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public Task<bool> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
