using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Connections.Features;
using RentalAttireBackend.Application.Common;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ITransactionManager _transaction;
        private readonly IPersonRepository _personRepo;

        public CreateEmployeeCommandHandler
            (
            IMapper mapper,
            IEmployeeRepository employeeRepo,
            ITransactionManager transaction,
            IPersonRepository personRepo
            )
        {
            _mapper = mapper;
            _employeeRepo = employeeRepo;
            _transaction = transaction;
            _personRepo = personRepo;
        }

        public async Task<Result<bool>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request is null || request.Person is null)
                return Result<bool>.Failure("Invalid request. Please fill all the required fields.");

            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);
                var person = _mapper.Map<Person>(request.Person);

                var createPerson = await _personRepo.CreatePersonAsync(person, cancellationToken);

                if(createPerson == 0)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee cannot be registered. Please try again.");
                }

                var employee = _mapper.Map<Employee>(request);
                employee.Person = person;
                employee.PersonId = createPerson;
                employee.EntityType = "Employee";

                var createEmployee = await _employeeRepo.CreateEmployeeAsync(employee, cancellationToken);

                if (!createEmployee)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee cannot be registered. Please try again.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Employee successfully registered!");
            }catch(Exception e)
            {
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
