using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        #region Commands
        public Task<int> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        public Task<bool> UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        #endregion

        #region Queries
        public Task<PagedResult<Employee>> GetAllEmployeesAsync(PaginationParams paginationParams ,CancellationToken cancellationToken);
        public Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
        public Task<Employee?> GetEmployeeByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken);
        public Task<PagedResult<Employee>> SearchEmployeeAsync(
            string searchQuery, PaginationParams paginationParams, CancellationToken cancellationToken
            );
        #endregion
    }
}
