namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalPageRequest
    {
        public string? CategoryType { get; set; } = string.Empty;
        public string? SearchQuery { get; set; } = string.Empty;
        public DateTime? StartingDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndingDate { get; set; } = DateTime.UtcNow;
        public int CurrentPage { get; set; } 
        public int ItemsPerPage { get; set; }
    }
}
