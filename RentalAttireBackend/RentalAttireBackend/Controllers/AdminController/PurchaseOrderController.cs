using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using RentalAttireBackend.Application.PurchaseOrders.Queries.FilterPurchaseOrders;
using RentalAttireBackend.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

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

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrderAsync(CreatePurchaseOrderCommand command)
        {
            var result = await _mediator.Send(command);

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderByIdAsync(int id)
        {
            var result = await _mediator.Send(new GetPurchaseOrderByIdQuery { PurchaseOrderId = id });

            return result.ToActionResult(this, _httpContextAccessor);
        }
    }
}
