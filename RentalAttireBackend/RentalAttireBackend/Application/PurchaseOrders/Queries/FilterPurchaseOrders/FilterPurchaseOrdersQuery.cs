using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.PurchaseOrders.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.PurchaseOrders.Queries.FilterPurchaseOrders
{
    public class FilterPurchaseOrdersQuery : IRequest<Result<PagedResult<PurchaseOrderDTO>>>
    {
        public string? SearchQuery { get; set; } = string.Empty;
        public List<string> Statuses { get; set; } = new();
        public string? DateTypeToggle { get; set; } = string.Empty;
        public DateTime? StartingDate { get; set; } = null;
        public DateTime? EndingDate { get; set; } = null;
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
