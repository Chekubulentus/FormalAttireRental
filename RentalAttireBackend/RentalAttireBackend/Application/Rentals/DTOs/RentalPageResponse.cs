using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalPageResponse
    {
        public List<RentalDTO> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)ItemsPerPage);
        public double TotalRevenue { get; set; }

        public Dictionary<string, int> StatusCounts { get; set; } = new();
        public int OverdueCount { get; set; }
        public int DueSoonCout { get; set; }
    }
}
