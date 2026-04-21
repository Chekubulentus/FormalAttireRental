using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.CustomerRestorer
{
    public class CustomerRestorer : IEntityRestorer
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IAuditLogService _auditService;

        public CustomerRestorer(ICustomerRepository customerRepo, IAuditLogService auditService)
        {
            _customerRepo = customerRepo;
            _auditService = auditService;
        }

        public string EntityType => "Customer";

        public async Task<Result<bool>> RestoreAsync(
            int entityId, 
            string performedBy,
            int performedById,
            CancellationToken ct)
        {
            if (entityId == 0)
                return Result<bool>.Failure("Invalid request.");

            var customerToRestore = await _customerRepo.GetCustomerByIdAsync(entityId, ct);

            if (customerToRestore is null)
                return Result<bool>.Failure("Customer does not exist.");

            var customerUser = customerToRestore.User;
            var customerPerson = customerToRestore.User.Person;

            customerToRestore.IsActive = true;
            customerToRestore.RestoredAt = DateTime.UtcNow;
            customerToRestore.RestoredBy = performedBy;

            customerUser.IsActive = true;
            customerUser.RestoredAt = DateTime.UtcNow;
            customerUser.RestoredBy = performedBy;

            customerPerson.IsActive = true;
            customerPerson.RestoredAt = DateTime.UtcNow;
            customerPerson.RestoredBy = performedBy;

            var updateCustomer = await _customerRepo.UpdateCustomerAsync(customerToRestore, ct);

            if (!updateCustomer)
                return Result<bool>.Failure("Record could not be restored. Please try again.");

            var auditTransaction = await _auditService.RestorationAuditLogAsync(
                customerToRestore,
                performedBy,
                performedById,
                customerPerson.FullName
                );

            if (!auditTransaction)
                return Result<bool>.Failure("Transaction could not be audited.");

            return Result<bool>.SuccessWithMessage("Customer record successfully restored.");
        }
    }
}
