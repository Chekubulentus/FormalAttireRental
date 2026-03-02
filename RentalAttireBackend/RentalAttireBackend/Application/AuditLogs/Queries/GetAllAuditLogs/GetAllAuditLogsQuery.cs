using MediatR;
using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsQuery : IRequest<Result<AuditLogResponse>>
    {
        public string ActionType { get; set; } = string.Empty;
        public string? SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
