using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Clothes.Commands.CreateClothe;
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
        public async Task<IActionResult> CreateClotheAsync(CreateClotheCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
