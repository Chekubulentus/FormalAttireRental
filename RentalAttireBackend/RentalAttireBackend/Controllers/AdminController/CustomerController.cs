using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Clothes.Queries.FilterClothes;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Customers.Commands.ArchiveCustomer;
using RentalAttireBackend.Application.Customers.Commands.CustomerRegistration;
using RentalAttireBackend.Application.Customers.Commands.UpdateCustomerProfile;
using RentalAttireBackend.Application.Customers.Queries.FilterCustomers;
using RentalAttireBackend.Application.Customers.Queries.GetCustomerProfile;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
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

        [HttpPost]
        public async Task<IActionResult> CustomerRegistrationAsync(CustomerRegistrationCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("customer-profile")]
        public async Task<IActionResult> UpdateCustomerProfileAsync(UpdateCustomerProfileCommand command)
        {
            var result = await _mediator.Send(command);

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpGet("get-customer-profile")]
        public async Task<IActionResult> GetCustomerProfileAsync()
        {
            var result = await _mediator.Send(new GetCustomerProfileQuery());

            return result.ToActionResult(this, _httpContextAccessor);
        }
    }
}
