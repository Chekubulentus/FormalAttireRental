using Microsoft.Extensions.ObjectPool;

namespace RentalAttireBackend.Application.Rentals.DTOs
{
    public static class AllowedStatusConditions
    {
        public static readonly List<string> Statuses = new()
        {
            "Pending",
            "Confirmed",
            "Declined",
            "Ready for pickup",
            "Returned",
        };
        public static readonly List<string> RevenueStatuses = new()
        {
            "Confirmed",
            "Ready for pickup",
            "Returned"
        };
    }
}
