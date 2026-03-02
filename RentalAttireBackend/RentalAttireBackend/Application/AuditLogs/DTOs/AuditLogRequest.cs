    namespace RentalAttireBackend.Application.AuditLogs.DTOs
{
    public class AuditLogRequest
    {
        public string? ActionType { get; set; }
        public string? SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
