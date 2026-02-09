namespace RentalAttireBackend.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; } //identifier ng entity created, updated, or archived
        public string ActionType { get; set; } = string.Empty; //Values: Create, Update, Archived
        public string ChangedBy { get; set; } = string.Empty;
        public int? ChangedById { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? OldValues { get; set; } = string.Empty;
        public string? NewValues { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty; //IP address ng user na nang bago
    }
}
