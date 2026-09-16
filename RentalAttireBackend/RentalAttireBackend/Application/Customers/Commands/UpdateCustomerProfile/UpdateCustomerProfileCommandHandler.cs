using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Customers.Commands.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly ITransactionManager _transactionManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAuditLogService _auditService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPasswordHasher _passwordHasher;

        public UpdateCustomerProfileCommandHandler(
            IMapper mapper,
            IUserRepository userRepo,
            ITransactionManager transactionManager,
            IFileUploadService fileUploadService,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService,
            IPasswordHasher passwordHasher
            )
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _transactionManager = transactionManager;
            _fileUploadService = fileUploadService;
            _auditService = auditLogService;
            _currentUserService = currentUserService;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<bool>> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                var user = await _userRepo.GetUserWithPersonAsync(userId ?? 0, cancellationToken);

                if (user is null || user.Person is null)
                    return Result<bool>.FailureWithErrorType("User account does not exist.", ErrorType.NotFound);

                // OLD USER DETAILS FOR AUDIT LOG
                var oldUserDetails = await _userRepo.GetUserWithPersonNoTrackingAsync(userId ?? 0, cancellationToken);
                var oldPersoNDetails = oldUserDetails.Person;

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                if(!string.IsNullOrEmpty(request.NewPassword))
                {
                    if (!request.isPasswordMatched)
                        return Result<bool>.FailureWithErrorType("New password and confirm password does not match.", ErrorType.BadRequest);

                    if (!string.IsNullOrEmpty(user.HashedPassword) &&
                        !_passwordHasher.VerifyPassword(request.CurrentPassword, user.HashedPassword))
                        return Result<bool>.FailureWithErrorType("Current password is incorrect.", ErrorType.BadRequest);

                    user.HashedPassword = _passwordHasher.HashPassword(request.NewPassword);
                }

                // NEW USER DETAILS FOR AUDIT LOG
                _mapper.Map(request, user);
                _mapper.Map(request, user.Person);

                if(request.NewProfileImage is not null)
                {
                    await _fileUploadService.DeleteFileAsync(user.Person.ProfileImagePath);

                    var uploadImage = await _fileUploadService.UploadImageAsync(request.NewProfileImage, $"persons/{user.Person.Id}");

                    if (!uploadImage.Success)
                    {
                        await _transactionManager.RollbackTransactionAsync(cancellationToken);
                        return Result<bool>.Failure("Image could not be uploaded.");
                    }

                    user.Person.ProfileImagePath = uploadImage.FilePath;
                }

                var updateUser = await _userRepo.UpdateUserAsync(user, cancellationToken);

                if (!updateUser)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Profile edit failed", ErrorType.BadRequest);
                }

                var auditLog = await _auditService.UpdateAuditLogAsync(
                    oldUserDetails,
                    user,
                    user.Id,
                    user.Person.FullName,
                    user.Person.FullName
                    );

                if(!auditLog)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Failed to audit transaction. Please try again.", ErrorType.BadRequest);
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Profile successfully updated!");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                throw new Exception(e.Message);
            }
        }
    }
}
