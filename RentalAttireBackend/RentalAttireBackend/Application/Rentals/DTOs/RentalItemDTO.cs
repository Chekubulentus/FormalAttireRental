namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public class RentalItemDTO
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int ClotheId { get; set; }
        public int RentalPrice { get; set; }
        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
    }
}
