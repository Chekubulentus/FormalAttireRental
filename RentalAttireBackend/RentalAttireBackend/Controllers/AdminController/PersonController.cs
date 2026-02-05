using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Persons.Commands.UpdatePerson;
using RentalAttireBackend.Application.Persons.Commands.UploadImagePerson;
using RentalAttireBackend.Application.Persons.Queries.GetAllPeople;
using RentalAttireBackend.Application.Persons.Queries.GetPeopleByLastName;
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
        public async Task<IActionResult> GetAllPeopleAsync([FromBody]PaginationParams paginationParams)
        {
            var result = await _mediator.Send(new GetAllPeopleQuery {PaginationParams = paginationParams });

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonByIdAsync(int id)
        {
            var result = await _mediator.Send(new GetPersonByIdQuery { Id = id});

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePersonAsync(UpdatePersonCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.SuccessMessage) : BadRequest(result.ErrorMessage);
        }
        [HttpGet("last-name/{lastName}")]
        public async Task<IActionResult> GetPeopleByLastNameAsync(string lastName)
        {
            var result = await _mediator.Send(new GetPeopleByLastNameQuery { LastName = lastName });

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
        [HttpPost("person/{personId}/profile-image")]
        public async Task<IActionResult> UploadPersonProfileImage(int personId, [FromForm]IFormFile file)
        {
            var result = await _mediator.Send(new UploadImagePersonCommand { Image = file, Id = personId });

            return result.IsSuccess ? Ok(result.SuccessMessage) : BadRequest(result.ErrorMessage);
        }
    }
}
