using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsQueryHandler : IRequestHandler<GetAllAuditLogsQuery, Result<PagedResult<AuditLogDTO>>>
    {
        private readonly IAuditLogService _auditService;
        private readonly IMapper _mapper;

        public GetAllAuditLogsQueryHandler(
            IAuditLogService auditService,
            IMapper mapper
            )
        {
            _auditService = auditService;
            _mapper = mapper;
        }
        public async Task<Result<PagedResult<AuditLogDTO>>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
        {
            if (request is null || (request.CurrentPage == 0 && request.ItemsPerPage == 0))
                return Result<PagedResult<AuditLogDTO>>.Failure("Invalid request. Please try again.");
            try
            {
                var auditLogs = await _auditService.GetAllAuditLogsAsync(
                    request.ActionType,
                    request.SearchQuery,
                    request.CurrentPage,
                    request.ItemsPerPage,
                    request.DateFrom,
                    request.DateTo,
                    cancellationToken
                    );

                if (!auditLogs.Items.Any() || auditLogs.Items.Count() == 0)
                    return Result<PagedResult<AuditLogDTO>>.Failure("No audit log records exists.");

                var auditLogsDto = _mapper.Map<PagedResult<AuditLogDTO>>(auditLogs);

                return Result<PagedResult<AuditLogDTO>>.Success(auditLogsDto);
            }catch(Exception e)
            {
                return Result<PagedResult<AuditLogDTO>>.Failure(e.Message);
            }
        }
    }
}
