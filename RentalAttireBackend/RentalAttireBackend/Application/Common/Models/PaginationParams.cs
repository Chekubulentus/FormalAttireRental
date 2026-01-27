namespace RentalAttireBackend.Application.Common.Models
{
    public class PaginationParams
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }

        public int Skip => (CurrentPage - 1) * ItemsPerPage;
    }
}
