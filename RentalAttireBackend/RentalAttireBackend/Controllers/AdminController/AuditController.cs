using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RentalAttireBackend.Application.AuditLogs.Queries.GetAllAuditLogs;

namespace RentalAttireBackend.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditController(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuditLogsAsync(
            string actionType,
            string? searchQuery,
            int currentPage,
            int itemsPerPage,
            DateTime? dateTo,
            DateTime? dateFrom
            )
        {
            var result = await _mediator.Send(new GetAllAuditLogsQuery
            {
                ActionType = actionType,
                SearchQuery = searchQuery,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            return result.IsSuccess ? Ok(result) : BadRequest(result.ErrorMessage);
        }
    }
}
