using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.Suppliers.Queries.FilterSuppliers;
using RentalAttireBackend.Application.Suppliers.Queries.GetSupplierById;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        [HttpGet("filter-suppliers")]
        public async Task<IActionResult> FilterSupplierAsync(
            string? searchQuery,
            int currentPage,
            int itemsPerPage
            )
        {
            var result = await _mediator.Send(new FilterSuppliersQuery
            {
                SearchQuery = searchQuery,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            });

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierByIdAsync(int id)
        {
            var result = await _mediator.Send(new GetSupplierByIdQuery { Id = id });

            return result.ToActionResult(this, _httpContextAccessor);
        }
    }
}
