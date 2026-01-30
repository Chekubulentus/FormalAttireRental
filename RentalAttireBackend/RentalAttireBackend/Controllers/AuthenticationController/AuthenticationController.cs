using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Authentication.Commands.Login;

namespace RentalAttireBackend.Controllers.AuthenticationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }
    }
}
