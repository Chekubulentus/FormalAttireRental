using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Authentication.Commands.GoogleLogin;
using RentalAttireBackend.Application.Authentication.Commands.Login;
using RentalAttireBackend.Application.Authentication.Commands.RefreshToken;
using RentalAttireBackend.Application.Authentication.Commands.RegistrationCommand;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Controllers.AuthenticationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenticationController(IMediator mediator, IJwtTokenGenerator jwtTokenGenerator)
        {
            _mediator = mediator;
            _jwtTokenGenerator = jwtTokenGenerator;
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
    }
}
