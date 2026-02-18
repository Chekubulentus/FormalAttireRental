using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Users.DTO;
using RentalAttireBackend.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace RentalAttireBackend.Application.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IUserRepository userRepo,
            IJwtTokenGenerator tokenGenerator,
            IMapper mapper
            )
        {
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
        }
        public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
            var userEmail = principal.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;

            var user = await _userRepo.GetUserByEmailAsync(userEmail, cancellationToken);

            if (user is null)
                return Result<AuthenticationResult>.Failure("User does not exist.");

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Result<AuthenticationResult>.Failure("Invalid or expired refresh token.");

            var newAccessToken = _tokenGenerator.GenerateAccessToken(user);

            user.RefreshToken = _tokenGenerator.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepo.UpdateUserAsync(user, cancellationToken);

            var userDto = _mapper.Map<UserDTO>(user);

            return Result<AuthenticationResult>.Success(new AuthenticationResult
            {
                AccessToken = newAccessToken,
                RefreshToken = user.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = userDto
            });
        }
    }
}
