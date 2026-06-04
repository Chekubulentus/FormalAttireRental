using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Rentals.Queries;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IMediator _meaditor;

        public RentalController(IMediator mediator)
        {
            _meaditor = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> FilterRentalsAsync(
            string? categoryType,
            string? searchQuery,
            int currentPage,
            int itemsPerPage,
            DateTime? startingDate,
            DateTime? endingDate
        )
        {
            var result = await _meaditor.Send(new FilterRentalsQuery
            {
                CategoryType = categoryType,
                SearchQuery = searchQuery,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                StartingDate = startingDate,
                EndingDate = endingDate
            });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
