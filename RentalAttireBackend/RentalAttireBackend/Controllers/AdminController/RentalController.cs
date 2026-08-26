using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.Rentals.Commands.RentalTransaction;
using RentalAttireBackend.Application.Rentals.Commands.UpdateRentalStatusCommand;
using RentalAttireBackend.Application.Rentals.Queries;
using RentalAttireBackend.Application.Rentals.Queries.FilterRentals;
using RentalAttireBackend.Application.Rentals.Queries.GetAllRentals;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalController : ControllerBase
    {
        private readonly IMediator _meaditor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RentalController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _meaditor = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> FilterRentalsAsync(
            string? status,
            string? searchQuery,
            int currentPage,
            int itemsPerPage,
            DateTime? startingDate,
            DateTime? endingDate
        )
        {
            var result = await _meaditor.Send(new FilterRentalsQuery
            {
                Status = status,
                SearchQuery = searchQuery,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                StartingDate = startingDate,
                EndingDate = endingDate
            });

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpGet("all-rentals")]
        public async Task<IActionResult> GetAllRentalsAsync(GetAllRentalsQuery query)
        {
            var result = await _meaditor.Send(query);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> RentalReservationAsync(RentalTransactionCommand command)
        {
            var result = await _meaditor.Send(command);

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateRentalStatusAsync(UpdateRentalStatusCommmand command)
        {
            var result = await _meaditor.Send(command);

            return result.ToActionResult(this, _httpContextAccessor); 
        }
    }
}
