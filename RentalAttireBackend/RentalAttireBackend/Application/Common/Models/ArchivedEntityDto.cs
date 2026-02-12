namespace RentalAttireBackend.Application.Common.Models
{
    public class ArchivedEntityDto
    {
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public string ArchivedBy { get; set; } = string.Empty;
    }
}
