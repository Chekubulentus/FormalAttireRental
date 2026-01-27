namespace RentalAttireBackend.Domain.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string ArchivedBy { get; set; } = string.Empty;
        public DateTime? ArchivedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string? EntityType { get; set; } = string.Empty;
    }
}
