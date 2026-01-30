using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;

        public LoginCommandHandler
            (
            IUserRepository userRepo,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator,
            IMapper mapper
            )
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
        }
        public async Task<Result<AuthenticationResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
                return Result<AuthenticationResult>.Failure("Invalid request. Please fill in the required fields.");

            try
            {
                var user = await _userRepo.GetUserByEmailAsync(command.Email, cancellationToken);

                if (user is null)
                    return Result<AuthenticationResult>.Failure("Invalid email or password. Please try again.");

                var passwordValidation = _passwordHasher.VerifyPassword(command.Password, user.HashedPassword);

                if (!passwordValidation)
                    return Result<AuthenticationResult>.Failure("Invalid email or password. Please try again.");

                var accessToken = _tokenGenerator.GenerateAccessToken(user);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userRepo.UpdateUserAsync(user, cancellationToken);

                return Result<AuthenticationResult>.Success(new AuthenticationResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = _mapper.Map<UserDTO>(user)
                });
            }catch(Exception e)
            {
                return Result<AuthenticationResult>.Failure(e.Message);
            }
        }
    }
}
