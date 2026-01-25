using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Persons.Queries.GetAllPeople;
using RentalAttireBackend.Application.Persons.Queries.GetPersonById;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController
            (
            IMediator mediator
            )
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPeopleAsync(GetAllPeopleQuery command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonByIdAsync(GetPersonByIdQuery command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
    }
}
