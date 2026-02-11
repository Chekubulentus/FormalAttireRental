using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Authentication.Commands.Login;
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

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("test-token")]
        [AllowAnonymous]
        public IActionResult TestToken()
        {
            // Create a dummy user for testing
            var testUser = new User
            {
                Id = 1,
                Email = "test@example.com",
                Person = new Person { FirstName = "Test", LastName = "User" },
                Employee = new Employee
                {
                    Role = new Role { RolePosition = Enum.Parse<RolePosition>("Administrator") }
                }
            };

            var token = _jwtTokenGenerator.GenerateAccessToken(testUser);

            return Ok(new { token });
        }
    }
}
