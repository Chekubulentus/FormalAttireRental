using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Categories.Commands.ArchiveCategory;
using RentalAttireBackend.Application.Categories.Commands.CreateCategory;
using RentalAttireBackend.Application.Categories.Commands.UpdateCategory;
using RentalAttireBackend.Application.Categories.Queries.FilterCategories;
using RentalAttireBackend.Application.Categories.Queries.GetAllCategories;
using RentalAttireBackend.Application.Categories.Queries.GetCategoryById;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Controllers.ClothesManagerController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery());

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("filter-categories")]
        public async Task<IActionResult> FilterCategoriesAsync(
            string? searchQuery,
            int currentPage,
            int itemsPerPage
            )
        {
            var paginationParams = new PaginationParams
            {
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            };

            var query = new FilterCategoriesQuery
            {
                SearchQuery = searchQuery,
                PaginationParams = paginationParams
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPatch]
        public async Task<IActionResult> ArchiveCategoryByIdAsync(ArchiveCategoryByIdCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategoryAsync(UpdateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
