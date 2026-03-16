using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Clothes.Commands.ArchiveClothe;
using RentalAttireBackend.Application.Clothes.Commands.CreateClothe;
using RentalAttireBackend.Application.Clothes.Commands.UpdateClothe;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Clothes.Queries.FilterClothes;
using RentalAttireBackend.Application.Clothes.Queries.GetAllClothes;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Controllers.ClothesManagerController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClotheController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClotheController(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClothesAsync(int currentPage, int itemsPerPage)
        {
            var paginationParams = new PaginationParams
            {
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            };

            var result = await _mediator.Send(new GetAllClothesQuery { PaginationParams = paginationParams });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClotheAsync([FromForm] CreateClotheCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("filter-clothes")]
        public async Task<IActionResult> FilterClothesAsync(
            string? searchQuery,
            string? condition,
            string? gender,
            string? category,
            int currentPage,
            int itemsPerPage
            )
        {
            var paginationParams = new PaginationParams
            {
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage
            };

            var filterParameters = new ClothesFIlterParameters
            {
                SearchQuery = searchQuery,
                ClotheGender = gender,
                Condition = condition,
                Category = category,
                PaginationParams = paginationParams
            };

            var result = await _mediator.Send(new FilterClothesQuery { FilterParameters = filterParameters });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClotheAsync(UpdateClotheCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }

        [HttpPatch]
        public async Task<IActionResult> ArchiveClotheByIdAsync(ArchiveClotheCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
