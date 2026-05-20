using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Diagnostics;
using System.Reflection;

namespace RentalAttireBackend.Application.Disposables.EntityDeleters
{
    public class CustomerDeleter : IDeleteArchivedEntity
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IAuditLogService _auditService;

        public CustomerDeleter(
            ICustomerRepository customerRepo,
            IAuditLogService auditService
            )
        {
            _customerRepo = customerRepo;
            _auditService = auditService;
        }
        public string EntityType => "Customer";

        public async Task<Result<bool>> DeleteArchivedRecordAsync(int id, string performedBy, int performedById, CancellationToken ct)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(id, ct);

            if (customer is null)
                return Result<bool>.Failure("Record could not be found.");

            customer.IsDeleted = true;
            customer.User.IsDeleted = true;
            customer.User.Person.IsDeleted = true;

            var deleteCustomer = await _customerRepo.UpdateCustomerAsync(customer, ct);

            if (!deleteCustomer)
                return Result<bool>.Failure("Record could not be deleted. Please try again.");

            var auditTransaction = await _auditService.DeleteAuditLogAsync(
                customer,
                performedBy,
                performedById,
                customer.User.Person.FullName
                );

            if (!auditTransaction)
                return Result<bool>.Failure("Transaction could not be audited.");

            return Result<bool>.SuccessWithMessage("Record permanently deleted.");
        }
    }
}
