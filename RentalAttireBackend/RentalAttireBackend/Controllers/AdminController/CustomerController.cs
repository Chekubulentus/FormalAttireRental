using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Clothes.Queries.FilterClothes;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.Commands.ArchiveCustomer;
using RentalAttireBackend.Application.Customers.Queries.FilterCustomers;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }
        [HttpGet("filter-customers")]
        public async Task<IActionResult> FilterClothesAsync(
            int currentPage,
            int itemsPerPage,
            string? searchQuery
            )
        {
            var paginationParams = new PaginationParams
            {
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            };

            var result = await _mediator.Send(new FilterCustomersQuery {PaginationParams = paginationParams, SearchQuery = searchQuery });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPatch]
        public async Task<IActionResult> ArchiveCustomerByIdAsync(ArchiveCustomerByIdCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
