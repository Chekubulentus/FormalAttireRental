using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsQueryHandler : IRequestHandler<GetAllAuditLogsQuery, Result<AuditLogResponse>>
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
        public async Task<Result<AuditLogResponse>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
        {
            if (request is null || (request.CurrentPage == 0 && request.ItemsPerPage == 0))
                return Result<AuditLogResponse>.Failure("Invalid request. Please try again.");
            try
            {
                var auditResponse = await _auditService.GetAllAuditLogsAsync(
                    request.ActionType,
                    request.SearchQuery,
                    request.CurrentPage,
                    request.ItemsPerPage,
                    request.DateFrom,
                    request.DateTo,
                    cancellationToken
                    );

                if (!auditResponse.Logs.Any() || auditResponse.Logs.Count() == 0)
                    return Result<AuditLogResponse>.Failure("Empty audit logs.");

                return Result<AuditLogResponse>.Success(auditResponse);
            }catch(Exception e)
            {
                return Result<AuditLogResponse>.Failure(e.Message);
            }
        }
    }
}
