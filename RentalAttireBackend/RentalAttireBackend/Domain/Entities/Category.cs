using RentalAttireBackend.Domain.Common;

namespace RentalAttireBackend.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //NavProp
        public List<Clothe> Clothes { get; set; } = new();
    }
}
