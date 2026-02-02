using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Employees.Commands.CreateEmployee;
using RentalAttireBackend.Application.Employees.Queries.GetAllEmployees;
using RentalAttireBackend.Application.Employees.Queries.GetEmployeeById;
using RentalAttireBackend.Application.Employees.Queries.SearchEmployee;

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
        public async Task<IActionResult> GetAllEmployeesAsync([FromQuery] PaginationParams paginationParams)
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery { PaginationParams = paginationParams});

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.SuccessMessage) : BadRequest(result.ErrorMessage);
        }
        [HttpGet("search-employee")]
        public async Task<IActionResult> SearchEmployeeAsync([FromBody]SearchEmployeeQuery query)
        {
            var result = await _mediator.Send(new SearchEmployeeQuery { SearchQuery = query.SearchQuery, 
                PaginationParams = query.PaginationParams});

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(int id)
        {
            var result = await _mediator.Send(new GetEmployeeByIdQuery { Id = id});

            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }
    }
}
