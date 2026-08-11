using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Authentication.Commands.ProfileCompletion
{
    public class ProfileCompletionCommandHandler : IRequestHandler<ProfileCompletionCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ITransactionManager _transactionManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public ProfileCompletionCommandHandler(
            IUserRepository userRepo,
            ITransactionManager transactionManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            )
        {
            _userRepo = userRepo;
            _transactionManager = transactionManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Result<bool>> Handle(ProfileCompletionCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            var userIdClaim = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirst("userId")
                ?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Result<bool>.Failure("Current user does not exist.");

            try
            {
                var user = await _userRepo.GetUserByIdWithCustomerAsync(userId, cancellationToken);

                if (user is null)
                    return Result<bool>.Failure("Current user could not be found.");

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var updatedPersonProfile = _mapper.Map(request, user.Person);

                var updateProfile = await _userRepo.UpdateUserAsync(user, cancellationToken);

                if(!updateProfile)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Profile cannot be updated. Please try again.");
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Profile completion complete!");
            }catch(Exception e)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
