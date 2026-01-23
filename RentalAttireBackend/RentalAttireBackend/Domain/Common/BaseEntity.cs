namespace RentalAttireBackend.Domain.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string ArchivedBy { get; set; } = string.Empty;
        public DateTime? ArchivedAt { get; set; } 
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? EntityType { get; set; } = string.Empty;
    }
}
