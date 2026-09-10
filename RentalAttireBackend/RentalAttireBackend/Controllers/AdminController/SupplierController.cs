using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Extensions;
using RentalAttireBackend.Application.Suppliers.Commands.CreateSupplier;
using RentalAttireBackend.Application.Suppliers.Commands.UpdateSupplier;
using RentalAttireBackend.Application.Suppliers.Queries.AssignClothesModal;
using RentalAttireBackend.Application.Suppliers.Queries.FilterSuppliers;
using RentalAttireBackend.Application.Suppliers.Queries.GetSupplierById;
using RentalAttireBackend.Application.Suppliers.Queries.GetSupplierClothesById;

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

        [HttpPost]
        public async Task<IActionResult> CreateSupplierAsync(CreateSupplierCommand command)
        {
            var result = await _mediator.Send(command);

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpPut] 
        public async Task<IActionResult> UpdateSupplierAsync(UpdateSupplierCommand command)
        {
            var result = await _mediator.Send(command);

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpGet("supplier-clothes")]
        public async Task<IActionResult> GetSupplierClothesByIdAsync(int id, int currentPage, int itemsPerPage)
        {
            var result = await _mediator.Send(new GetSupplierClothesByIdQuery
            {
                Id = id,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            });

            return result.ToActionResult(this, _httpContextAccessor);
        }

        [HttpGet("assignable-clothes")]
        public async Task<IActionResult> FilterAssignableClothesAsync(
            int? supplierId,
            string? searchQuery,
            string? category,
            string? gender,
            int currentPage,
            int itemsPerPage
            )
        {
            var result = await _mediator.Send(new AssignClothesModalQuery
            {
                SupplierId = supplierId,
                SearchQuery = searchQuery,
                Category = category,
                Gender = gender,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            });

            return result.ToActionResult(this, _httpContextAccessor);
        }
    }
}
