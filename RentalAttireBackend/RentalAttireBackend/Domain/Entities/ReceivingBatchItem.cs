using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class ReceivingBatchItem : BaseEntity
    {
        public int ReceivingBatchId { get; set; }
        public int PurchaseOrderItemId { get; set; } 
        public int QuantityReceived { get; set; }

        // Nav Prop
        [JsonIgnore]
        public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
    }
}
