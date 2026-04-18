using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.CompilerServices;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.EmployeeRestorer
{
    public class EmployeeRestorer : IEntityRestorer
    {
        private readonly IEmployeeRepository _repo;
        public EmployeeRestorer(IEmployeeRepository repo) => _repo = repo;
        public string EntityType => "Employee";

        public async Task<Result<bool>> RestoreAsync(int entityId, CancellationToken ct)
        {
            var employeeToRestore = await _repo.GetEmployeeByIdAsync(entityId, ct);

            if (employeeToRestore is null)
                return Result<bool>.Failure("Employee does not exist.");

            employeeToRestore.IsActive = true;
            employeeToRestore.User.IsActive = true;
            employeeToRestore.User.Person.IsActive = true;

            var updateRecord = await _repo.UpdateEmployeeAsync(employeeToRestore, ct);

            if (!updateRecord)
                return Result<bool>.Failure("Record could not be restored. Please try again.");

            return Result<bool>.SuccessWithMessage("Record successfully restored.");
        }
    }
}
