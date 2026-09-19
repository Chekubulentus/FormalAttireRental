using RentalAttireBackend.Application.Clothes.DTOs;

namespace RentalAttireBackend.Application.PurchaseOrderItems.DTOs
{
    public class PurchaseOrderItemDTO
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public ClotheDTO Clothe { get; set; } = null!;
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public double UnitCost { get; set; }
        public int OriginalSupplierId { get; set; }
        public double TotalAmount { get; set; }
    }
}
