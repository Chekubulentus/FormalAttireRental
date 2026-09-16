using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<bool>>
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IMapper _mapper;
        private readonly ITransactionManager _transaction;
        private readonly IAuditLogService _auditLogService;
        public UpdateEmployeeCommandHandler
            (
            IEmployeeRepository employeeRepo,
            IMapper mapper,
            ITransactionManager transaction,
            IAuditLogService auditLogService
            )
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
            _transaction = transaction;
            _auditLogService = auditLogService;
        }

        public async Task<Result<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request. Please try again.");
            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);
                var existingEmployee = await _employeeRepo.GetEmployeeByIdAsync(request.Id, cancellationToken);

                if (existingEmployee is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee does not exist. Please try again.");
                }

                var oldEmployeeDetails = await _employeeRepo.GetEmployeByIdNoTrackingAsync(request.Id, cancellationToken);
                var oldPersonDetails = oldEmployeeDetails?.User.Person;

                _mapper.Map(request, existingEmployee);
                _mapper.Map(request.Person, existingEmployee.User.Person);

                var auditEmployee = await _auditLogService.UpdateAuditLogAsync(
                    oldEmployeeDetails,
                    existingEmployee,
                    request.PerformedById,
                    request.PerformedBy,
                    existingEmployee.User.Person.FullName
                    );

                var auditPerson = await _auditLogService.UpdateAuditLogAsync(
                    oldPersonDetails,
                    existingEmployee.User.Person,
                    request.PerformedById,
                    request.PerformedBy,
                    existingEmployee.User.Person.FullName
                    );

                if(!auditEmployee || !auditPerson)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("No changes have been made to the employee.");
                }

                var updateEmployee = await _employeeRepo.UpdateEmployeeAsync(existingEmployee, cancellationToken);

                if (!updateEmployee)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Failed to update employee record. No changes were saved.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Employee successfully updated!");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
