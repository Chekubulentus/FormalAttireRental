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
        public int TotalRevenue { get; set; } 
    }
}
