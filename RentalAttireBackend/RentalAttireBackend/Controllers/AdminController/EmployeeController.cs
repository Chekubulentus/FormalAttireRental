using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Employees.Queries.GetAllEmployees;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController
            (
            IMediator mediator
            )
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeesAsync()
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery());

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
    }
}
