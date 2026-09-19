using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.PurchaseOrders.DTOs;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.PurchaseOrders.Queries.FilterPurchaseOrders
{
    public class FIlterPurchaseOrdersQueryHandler : IRequestHandler<FilterPurchaseOrdersQuery, Result<PagedResult<PurchaseOrderDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public FIlterPurchaseOrdersQueryHandler(
            IMapper mapper,
            IPurchaseOrderRepository purchaseOrderRepository
            )
        {
            _mapper = mapper;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<Result<PagedResult<PurchaseOrderDTO>>> Handle(FilterPurchaseOrdersQuery request, CancellationToken cancellationToken)
        {
            var pageResult = await _purchaseOrderRepository.FilterPurchaseOrdersAsync(
                request.SearchQuery,
                request.Statuses,
                request.DateTypeToggle,
                request.StartingDate,
                request.EndingDate,
                request.CurrentPage,
                request.ItemsPerPage,
                cancellationToken
                );

            //TODO: get all purchase order id then fetch employee name.

            var pageResultDto = _mapper.Map<PagedResult<PurchaseOrderDTO>>(pageResult);

            return Result<PagedResult<PurchaseOrderDTO>>.Success(pageResultDto);
        }
    }
}
