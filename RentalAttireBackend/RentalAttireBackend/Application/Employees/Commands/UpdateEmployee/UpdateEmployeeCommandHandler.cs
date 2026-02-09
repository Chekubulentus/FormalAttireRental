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
                var oldEmployee = await _employeeRepo.GetEmployeeByIdAsync(request.Id, cancellationToken);

                if (oldEmployee is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee does not exist. Please try again.");
                }

                var oldEmployeeDetails = _mapper.Map<Employee>(oldEmployee);
                var newEmployeeDetails = _mapper.Map<Employee>(request);

                _mapper.Map(request, oldEmployee);

                var oldPersonDetails = _mapper.Map<Person>(oldEmployee.User.Person);
                var newPersonDetails = _mapper.Map<Person>(request.Person);

                _mapper.Map(request.Person, oldEmployee.User.Person);

                var logEmployee = await _auditLogService.UpdateAuditLogAsync(
                    oldEmployeeDetails,
                    newEmployeeDetails,
                    request.PerformedById,
                    request.UpdatedBy);

                var logPerson = await _auditLogService.UpdateAuditLogAsync(
                    oldPersonDetails,
                    newPersonDetails,
                    request.PerformedById,
                    request.UpdatedBy
                    );

                if (!logEmployee || !logPerson)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("No changes have been made. Please try again.");
                }

                var updateEmployee = await _employeeRepo.UpdateEmployeeAsync(oldEmployee, cancellationToken);

                if (!updateEmployee)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee cannot be updated. Please try again.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Employee successfully updated!");
            }catch(Exception e)
            {
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
