using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Clothes.DTOs
{
    public class ClothesFIlterParameters
    {
        public string SearchQuery { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string ClotheGender { get; set; } = string.Empty;
        public PaginationParams PaginationParams { get; set; } = new();
    }
}
