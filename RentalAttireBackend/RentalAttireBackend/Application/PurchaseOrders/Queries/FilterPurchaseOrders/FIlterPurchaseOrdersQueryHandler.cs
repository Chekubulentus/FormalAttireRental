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
            if (request is null)
                return Result<PagedResult<PurchaseOrderDTO>>.FailureWithErrorType("Invalid request", ErrorType.BadRequest);

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

            var pageResultDto = _mapper.Map<PagedResult<PurchaseOrderDTO>>(pageResult);

            var poIds = pageResultDto.Items.Select(x => x.Id).ToList();

            var getPurchaseOrderEmployees = await _purchaseOrderRepository.GetAllPurchaseOrderEmployeeNames(poIds, cancellationToken);

            foreach(var po in pageResultDto.Items)
            {
                po.EmployeeName = getPurchaseOrderEmployees.GetValueOrDefault(po.Id, "N/A");
            }

            return Result<PagedResult<PurchaseOrderDTO>>.Success(pageResultDto);
        }
    }
}
