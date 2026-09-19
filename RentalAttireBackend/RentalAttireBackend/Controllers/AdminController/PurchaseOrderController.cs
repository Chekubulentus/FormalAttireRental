using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.PurchaseOrders.Queries.FilterPurchaseOrders;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseOrderController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("filter-purchase-orders")]
        public async Task<IActionResult> FilterPurchaseOrdersAsync(
            string? searchQuery,
            [FromQuery] List<string> statuses,
            string? dateTypeToggle,
            DateTime? startingDate,
            DateTime? endingDate,
            int currentPage,
            int itemsPerPage
            )
        {
            var result = await _mediator.Send(new FilterPurchaseOrdersQuery
            {
                SearchQuery = searchQuery,
                Statuses = statuses,
                DateTypeToggle = dateTypeToggle,
                StartingDate = startingDate,
                EndingDate = endingDate,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            });

            return result.ToActionResult(this, _httpContextAccessor);
        } 
    }
}
