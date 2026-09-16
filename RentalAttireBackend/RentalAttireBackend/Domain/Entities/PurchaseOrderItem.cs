using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class PurchaseOrderItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; } 
        public int ClotheId { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public double UnitCost { get; set; }
        public int OriginalSupplierId { get; set; }
        public double TotalAmount => OrderedQuantity * UnitCost;

        //Nav Prop
        [JsonIgnore]
        public Clothe Clothe { get; set; } = null!;
        [JsonIgnore]
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        [JsonIgnore]
        public List<ReceivingBatchItem> ReceivingBatchItems { get; set; } = new();
    }
}
