using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.PurchaseOrders.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.PurchaseOrders.Queries.GetPurchaseOrderById
{
    public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public GetPurchaseOrderByIdQueryHandler(
            IMapper mapper,
            IPurchaseOrderRepository purchaseOrderRepository,
            IEmployeeRepository employeeRepository
            )
        {
            _mapper = mapper;
            _purchaseOrderRepository = purchaseOrderRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<PurchaseOrderDTO>> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<PurchaseOrderDTO>.FailureWithErrorType(
                    "Invalid request",
                    ErrorType.BadRequest
                    );

            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderByIdAsync(request.PurchaseOrderId, cancellationToken);

            if (purchaseOrder is null)
                return Result<PurchaseOrderDTO>.FailureWithErrorType(
                    "Purchase order record could not be found",
                    ErrorType.NotFound
                    );

            var employee = await _employeeRepository.GetEmployeeByIdAsync(purchaseOrder.EmployeeId, cancellationToken);

            if (employee is null)
                return Result<PurchaseOrderDTO>.FailureWithErrorType(
                    $"Employee record associated with purchase order {purchaseOrder.PurchaseOrderCode} could not be found",
                    ErrorType.NotFound
                    );

            var purchaseOrderDto = _mapper.Map<PurchaseOrderDTO>(purchaseOrder);
            purchaseOrderDto.EmployeeName = employee.User.Person.FullName;

            return Result<PurchaseOrderDTO>.Success(purchaseOrderDto);
        }
    }
}
