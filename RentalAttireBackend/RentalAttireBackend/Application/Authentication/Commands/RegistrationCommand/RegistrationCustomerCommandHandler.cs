using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.DTOs;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace RentalAttireBackend.Application.Authentication.Commands.RegistrationCommand
{
    public class RegistrationCustomerCommandHandler : IRequestHandler<RegistrationCustomerCommand, Result<AuthenticationResult>>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ITransactionManager _transactionManager;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuditLogService _auditService;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepo;

        public RegistrationCustomerCommandHandler(
            ICustomerRepository customerRepo,
            ITransactionManager transactionManager,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IAuditLogService auditService,
            IJwtTokenGenerator tokenGenerator,
            IUserRepository userRepo
            )
        {
            _customerRepo = customerRepo;
            _transactionManager = transactionManager;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
            _tokenGenerator = tokenGenerator;
            _userRepo = userRepo;
        }

        public async Task<Result<AuthenticationResult>> Handle(RegistrationCustomerCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<AuthenticationResult>.Failure("Invalid request.");

            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                //CHECK USERNAME DUPLCIATION FIRST
                //PASSWORD CONFIRMATION
                var userDuplication = await _userRepo.ValidateEmailDuplicationAsync(request.Email, cancellationToken);

                if (!request.Password.Equals(request.ConfirmPassword))
                    return Result<AuthenticationResult>.Failure("Password and confirm password doest not match.");

                if (userDuplication)
                    return Result<AuthenticationResult>.Failure("Username already exist.");

                var hashedPassword = _passwordHasher.HashPassword(request.Password);

                var newCustomer = _mapper.Map<Customer>(request);
                newCustomer.User.HashedPassword = hashedPassword;
                newCustomer.CustomerCode = GenerateCustomerCode(request.Email);

                var createCustomer = await _customerRepo.CreateCustomerAsync(newCustomer, cancellationToken);

                if (!createCustomer)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<AuthenticationResult>.Failure("Customer could not be created.");
                }


                var accessToken = _tokenGenerator.GenerateAccessToken(newCustomer.User);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                newCustomer.User.RefreshToken = refreshToken;
                newCustomer.User.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                var insertTokens = await _userRepo.UpdateUserAsync(newCustomer.User, cancellationToken);

                if(!insertTokens)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<AuthenticationResult>.Failure("Tokens could bot be inserted.");
                }

                var auditTransaction = await _auditService.CreateAuditLogAsync(
                    newCustomer,
                    newCustomer.Id,
                    newCustomer.User.Person.FullName,
                    newCustomer.User.Person.FullName
                    );

                if(!auditTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<AuthenticationResult>.Failure("Transaction could not be audited.");
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<AuthenticationResult>.Success(new AuthenticationResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    Id = newCustomer.User.Id,
                    Email = newCustomer.User.Email,
                });
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<AuthenticationResult>.Failure(e.Message);
            }
        }
        private static string GenerateCustomerCode(string email)
        {
            var prefix = email.Split('@')[0]
                [..Math.Min(4, email.Split('@')[0].Length)]
                .ToUpper();
            var suffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"CUST-{prefix}-{suffix}";
        }
    }
}
