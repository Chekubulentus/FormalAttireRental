using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        #region Commands
        public Task<bool> CreateEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        #endregion

        #region Queries

        #endregion
    }
}
