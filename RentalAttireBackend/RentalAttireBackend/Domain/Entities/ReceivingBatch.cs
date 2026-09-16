using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class ReceivingBatch : BaseEntity
    {
        public string ReceivingBatchCode { get; set; } = string.Empty;
        public int PurchaseOrderId { get; set; }
        public string ReceiptImageFilePath { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
        public List<ReceivingBatchItem> ReceivingBatchItems { get; set; } = new();
        public int EmployeeId { get; set; } 

        // Nav Prop
        [JsonIgnore]
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        [JsonIgnore]
        public Employee Employee { get; set; } = null!;
    } 
}
