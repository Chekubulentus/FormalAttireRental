using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Users.Queries.GetUserViewModel
{
    public class GetUserViewModelByIdQueryHandler : IRequestHandler<GetUserViewModelByIdQuery, Result<UserViewModel>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        public GetUserViewModelByIdQueryHandler(
            IMapper mapper,
            IUserRepository userRepo
            )
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<Result<UserViewModel>> Handle(GetUserViewModelByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
                return Result<UserViewModel>.Failure("Invalid user identifier. Please try again.");
            try
            {
                var user = await _userRepo.GetUserModelViewByIdAsync(request.Id, cancellationToken);
                if (user is null)
                    return Result<UserViewModel>.Failure("User does not exist.");

                var userViewModel = _mapper.Map<UserViewModel>(user);

                return Result<UserViewModel>.Success(userViewModel);

            }catch(Exception e)
            {
                return Result<UserViewModel>.Failure(e.Message);
            }
        }
    }
}
