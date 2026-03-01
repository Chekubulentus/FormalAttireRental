namespace RentalAttireBackend.Application.AuditLogs.DTOs
{
    public class AuditLogDTO
    {
        public int Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}
