using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Customers.Commands.CustomerRegistration
{
    public class CustomerRegistrationCommandHandler : IRequestHandler<CustomerRegistrationCommand, Result<AuthenticationResult>>
    {
        private readonly IAuditLogService _auditService;
        private readonly ICustomerRepository _customerRepo;
        private readonly ITransactionManager _transactionManager;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;

        public CustomerRegistrationCommandHandler(
            IAuditLogService auditService,
            ICustomerRepository customerRepo,
            ITransactionManager transactionManager,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator,
            IMapper mapper
            )
        {
            _auditService = auditService;
            _customerRepo = customerRepo;
            _transactionManager = transactionManager;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthenticationResult>> Handle(CustomerRegistrationCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<AuthenticationResult>.Failure("Invalid request.");

            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                if (!request.Password.Equals(request.ConfirmPassword))
                    return Result<AuthenticationResult>.Failure("Password does not match Confirm Password.");

                var hashedPassword = _passwordHasher.HashPassword(request.Password);

                var newCustomer = _mapper.Map<Customer>(request);

                var accessToken = _tokenGenerator.GenerateAccessToken(newCustomer.User);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                newCustomer.User.RefreshToken = refreshToken;
                newCustomer.User.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                newCustomer.CustomerCode = GenerateCustomerCode(newCustomer.User.Email);

                var createCustomer = await _customerRepo.CreateCustomerAsync(newCustomer, cancellationToken);

                if (!createCustomer)
                    return Result<AuthenticationResult>.Failure("Failed to create customer record.");

                var auditTransaction = await _auditService.CreateAuditLogAsync(
                    newCustomer,
                    request.PerformedById,
                    request.PerformedBy,
                    newCustomer.User.Person.FullName
                    );

                if(!auditTransaction)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<AuthenticationResult>.Failure("Failed to created audit log for this action. No changes were saved.");
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<AuthenticationResult>.Success(new AuthenticationResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    Id = newCustomer.User.Id,
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
