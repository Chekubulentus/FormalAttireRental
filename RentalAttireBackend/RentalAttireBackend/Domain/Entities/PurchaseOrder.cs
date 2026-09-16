using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class PurchaseOrder : BaseEntity
    {
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpectedDeliveryDate { get; set; }
        public int SupplierId { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Draft;
        public List<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new();
        public int EmployeeId { get; set; } // So the admins can view who issued the PO
        public double TotalAmount => PurchaseOrderItems.Any() ? PurchaseOrderItems.Sum(x => x.TotalAmount) : 0.00;

        //Nav Prop
        [JsonIgnore]
        public Supplier Supplier { get; set; } = null!;
        [JsonIgnore]
        public Employee Employee { get; set; } = null!;

    }

    public enum OrderStatus  
    { 
        Draft = 0,
        Ordered = 1,
        PartiallyReceived = 2,
        Received = 3,
        Cancelled = 4
    }
}
