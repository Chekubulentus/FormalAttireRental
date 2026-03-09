using RentalAttireBackend.Domain.Common;
using System.Text.Json.Serialization;

namespace RentalAttireBackend.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //NavProp
        [JsonIgnore]
        public List<Clothe>? Clothes { get; set; } = new();
    }
}
