using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Users.Queries.GetUserViewModel;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(
            IMediator mediator
            )
        {
            _mediator = mediator;            
        }
        [HttpGet("user-view-model/{id}")]
        public async Task<IActionResult> GetUserViewModelByIdAsync(int id)
        {
            var result = await _mediator.Send(new GetUserViewModelByIdQuery { Id = id });

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
    }
}
