using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Connections.Features;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.DTOs;
using RentalAttireBackend.Application.Persons.DTO;
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
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public CreateEmployeeCommandHandler
            (
            IMapper mapper,
            IEmployeeRepository employeeRepo,
            ITransactionManager transaction,
            IPersonRepository personRepo,
            IUserRepository userRepo,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator
            )
        {
            _mapper = mapper;
            _employeeRepo = employeeRepo;
            _transaction = transaction;
            _personRepo = personRepo;
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<bool>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request. Please try again");

            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);
                var emailValidation = await _userRepo.ValidateEmailDuplicationAsync(request.Email, cancellationToken);

                if (emailValidation)
                    return Result<bool>.Failure("Email already exist.");

                var person = _mapper.Map<Person>(request.Person);

                var personId = await _personRepo.CreatePersonAsync(person, cancellationToken);

                var hashedPassword = _passwordHasher.HashPassword(request.Password);

                var newUser = _mapper.Map<User>(request);
                newUser.HashedPassword = hashedPassword;
                newUser.Person = person;

                var createUser = await _userRepo.CreateUserAsync(newUser, cancellationToken);

                if (!createUser)
                    return Result<bool>.Failure("Account creation failed.");

                var employee = _mapper.Map<Employee>(request);
                employee.User = newUser;
                employee.EntityType = "Employee";

                var createEmployee = await _employeeRepo.CreateEmployeeAsync(employee, cancellationToken);

                if (createEmployee == 0)
                    return Result<bool>.Failure("Employee cannot be created. Please try again");

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Employee successfully created!");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
