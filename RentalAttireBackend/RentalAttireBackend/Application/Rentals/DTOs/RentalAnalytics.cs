namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalAnalytics
    {
        public Dictionary<string, int> StatusCounts { get; set; } = new();
        public int OverdueCount { get; set; }
        public int DueSoonCount { get; set; }
    }
}
