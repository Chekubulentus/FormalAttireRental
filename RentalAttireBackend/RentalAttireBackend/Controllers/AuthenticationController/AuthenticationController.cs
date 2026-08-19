using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Authentication.Commands.GoogleLogin;
using RentalAttireBackend.Application.Authentication.Commands.Login;
using RentalAttireBackend.Application.Authentication.Commands.ProfileCompletion;
using RentalAttireBackend.Application.Authentication.Commands.RefreshToken;
using RentalAttireBackend.Application.Authentication.Commands.RegistrationCommand;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using System.Reflection.Metadata.Ecma335;

namespace RentalAttireBackend.Controllers.AuthenticationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationController(
            IMediator mediator, 
            IJwtTokenGenerator jwtTokenGenerator,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _mediator = mediator;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLoginAsync(GoogleLoginCommand command)
        { 
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpPost("registration")]
        public async Task<IActionResult> CustomerRegistrationCommandAsync(RegistrationCustomerCommand command)
        {
            var result = await _mediator.Send(command);

             return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpPut("profile-completion")]
        public async Task<IActionResult> ProfileCompletionAsync(ProfileCompletionCommand command)
        {
            var result = await _mediator.Send(command);

            return result.ToActionResult(this, _httpContextAccessor);
        }
    }
}
