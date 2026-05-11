using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.GetAllArchivedEntities;
using RentalAttireBackend.Application.Disposables.RestoreRecord;
using RentalAttireBackend.Application.Disposables.ViewRecord;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisposablesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DisposablesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllArchivedRecordsAsync(int currentPage, int itemsPerPage)
        {
            var paginationParams = new PaginationParams { CurrentPage = currentPage, ItemsPerPage = itemsPerPage };

            var result = await _mediator.Send(new GetAllArchivedEntitiesQuery { PaginationParams = paginationParams });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpPatch]
        public async Task<IActionResult> RestoreArchivedRecordAsync(RestoreRecordCommand command)
        {
            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
        [HttpGet("view-record")]
        public async Task<IActionResult> ViewArchivedRecordAsync(int id, string entityType)
        {
            var result = await _mediator.Send(new ViewRecordCommand { Id = id, EntityType = entityType});

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
