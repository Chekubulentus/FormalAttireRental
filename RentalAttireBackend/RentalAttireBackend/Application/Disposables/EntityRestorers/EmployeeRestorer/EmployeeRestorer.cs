using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.CompilerServices;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.EmployeeRestorer
{
    public class EmployeeRestorer : IEntityRestorer
    {
        private readonly IEmployeeRepository _repo;
        private readonly IAuditLogService _auditService;
        public EmployeeRestorer(IEmployeeRepository repo, IAuditLogService auditService)
        {
            _repo = repo;
            _auditService = auditService;
        } 
        public string EntityType => "Employee";

        public async Task<Result<bool>> RestoreAsync(
            int entityId,
            string performedBy,
            int performedById,
            CancellationToken ct)
        {
            var employeeToRestore = await _repo.GetEmployeeByIdAsync(entityId, ct);

            if (employeeToRestore is null)
                return Result<bool>.Failure("Employee does not exist.");

            var employeeUser = employeeToRestore.User;
            var employeePerson = employeeToRestore.User.Person;

            employeeToRestore.IsActive = true;
            employeeToRestore.RestoredAt = DateTime.UtcNow;
            employeeToRestore.RestoredBy = performedBy;

            employeeUser.IsActive = true;
            employeeUser.RestoredAt = DateTime.UtcNow;
            employeeUser.RestoredBy = performedBy;

            employeePerson.IsActive = true;
            employeePerson.RestoredAt = DateTime.UtcNow;
            employeePerson.RestoredBy = performedBy;

            var updateRecord = await _repo.UpdateEmployeeAsync(employeeToRestore, ct);

            if (!updateRecord)
                return Result<bool>.Failure("Record could not be restored. Please try again.");

            var auditTransaction = await _auditService.RestorationAuditLogAsync(
                employeeToRestore,
                performedBy,
                performedById,
                employeePerson.FullName
                );

            return Result<bool>.SuccessWithMessage("Record successfully restored.");
        }
    }
}
