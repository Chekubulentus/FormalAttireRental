using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.CustomerRestorer
{
    public class CustomerRestorer : IEntityRestorer
    {
        private readonly ICustomerRepository _customerRepo;

        public CustomerRestorer(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public string EntityType => "Customer";

        public async Task<Result<bool>> RestoreAsync(int entityId, CancellationToken ct)
        {
            if (entityId == 0)
                return Result<bool>.Failure("Invalid request.");

            var customerToRestore = await _customerRepo.GetCustomerByIdAsync(entityId, ct);

            if (customerToRestore is null)
                return Result<bool>.Failure("Customer does not exist.");

            customerToRestore.IsActive = true;
            customerToRestore.User.IsActive = true;
            customerToRestore.User.Person.IsActive = true;

            var updateCustomer = await _customerRepo.UpdateCustomerAsync(customerToRestore, ct);

            if (!updateCustomer)
                return Result<bool>.Failure("Record could not be restored. Please try again.");

            return Result<bool>.SuccessWithMessage("Customer record successfully restored.");
        }
    }
}
