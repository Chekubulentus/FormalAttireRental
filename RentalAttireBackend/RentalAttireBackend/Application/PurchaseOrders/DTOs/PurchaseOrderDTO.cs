using RentalAttireBackend.Application.PurchaseOrderItems.DTOs;

namespace RentalAttireBackend.Application.PurchaseOrders.DTOs
{
    public class PurchaseOrderDTO
    {
        public int Id { get; set; }
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } 
        public DateTime ExpectedDeliveryDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public List<PurchaseOrderItemDTO> PurchaseOrderItems { get; set; } = new();
    }
}
