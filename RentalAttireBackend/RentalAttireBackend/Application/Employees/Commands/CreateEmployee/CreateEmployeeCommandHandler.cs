using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Connections.Features;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
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
            if (request is null || request.Person is null)
                return Result<bool>.Failure("Invalid request. Please fill all the required fields.");

            var emailDuplication = await _userRepo.ValidateEmailDuplicationAsync(request.Email, cancellationToken);

            if (emailDuplication)
                return Result<bool>.Failure("Email already exist. Please try again.");

            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);
                var person = _mapper.Map<Person>(request.Person);
                person.EntityType = "Person";

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

                if (createEmployee == 0)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Employee cannot be registered. Please try again.");
                }

                var newUser = new User
                {
                    Email = request.Email,
                    HashedPassword = _passwordHasher.HashPassword(request.Password),
                    EmployeeId = createEmployee,
                    EntityType = "User"
                };

                var createUser = await _userRepo.CreateUserAsync(newUser, cancellationToken);

                var accessToken = _tokenGenerator.GenerateAccessToken(newUser);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                newUser.RefreshToken = refreshToken;
                newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userRepo.UpdateUserAsync(newUser, cancellationToken);

                if (!createUser)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("User cannot be created. Please try again.");
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
