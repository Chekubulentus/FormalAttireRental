using AutoMapper;
using MediatR;
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
        public UpdateEmployeeCommandHandler
            (
            IEmployeeRepository employeeRepo,
            IMapper mapper,
            ITransactionManager transaction
            )
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
            _transaction = transaction;
        }

        public async Task<Result<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request. Please try again.");
            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);
                var employee = await _employeeRepo.GetEmployeeByIdAsync(request.Id, cancellationToken);

                if (employee is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee does not exist. Please try again.");
                }


                var updatedEmployee = _mapper.Map(request, employee);

                var person = updatedEmployee.User.Person;

                person.LastName = request.Person.LastName;
                person.FirstName = request.Person.FirstName;
                person.MiddleName = request.Person.MiddleName;
                person.Age = request.Person.Age;
                person.Gender = Enum.Parse<Gender>(request.Person.Gender, true);
                person.MaritalStatus = Enum.Parse<MaritalStatus>(request.Person.MaritalStatus, true);
                person.PhoneNumber = request.Person.PhoneNumber;
                person.Street = request.Person.Street;
                person.Barangay = request.Person.Barangay;
                person.City = request.Person.City;
                person.Province = request.Person.Province;
                person.PostalCode = request.Person.PostalCode;

                var updateEmployee = await _employeeRepo.UpdateEmployeeAsync(updatedEmployee, cancellationToken);

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
