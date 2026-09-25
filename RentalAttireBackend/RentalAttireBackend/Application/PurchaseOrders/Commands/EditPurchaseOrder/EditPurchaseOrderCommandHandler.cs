using AutoMapper;
using Google.Apis.Util;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.EditPurchaseOrder
{
    public class EditPurchaseOrderCommandHandler : IRequestHandler<EditPurchaseOrderCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly ITransactionManager _transactionManager;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IClotheRepository _clotheRepository;

        public EditPurchaseOrderCommandHandler(
            IMapper mapper,
            ITransactionManager transactionManager,
            IPurchaseOrderRepository purchaseOrderRepository,
            IEmployeeRepository employeeRepository,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService,
            ISupplierRepository supplierRepository,
            IClotheRepository clotheRepository
            )
        {
            _mapper = mapper;
            _transactionManager = transactionManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _employeeRepository = employeeRepository;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
            _supplierRepository = supplierRepository;
            _clotheRepository = clotheRepository;
        }
        public async Task<Result<bool>> Handle(EditPurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.FailureWithErrorType(
                    "Invalid request",
                    ErrorType.BadRequest
                    );

            var currentUserId = _currentUserService.UserId;

            var employee = await _employeeRepository.GetEmployeByIdNoTrackingAsync(currentUserId ?? 0, cancellationToken);

            if (employee is null)
                return Result<bool>.FailureWithErrorType(
                    "Employee associated with this action could not be found",
                    ErrorType.NotFound
                    );

            var purchaseOrder = await _purchaseOrderRepository.GetPurchaseOrderByIdAsync(request.PurchaseOrderId, cancellationToken);

            if (purchaseOrder is null)
                return Result<bool>.FailureWithErrorType(
                    "Purchase order ",
                    ErrorType.BadRequest
                    );

            var supplier = await _supplierRepository.GetSupplierByIdAsyncNoTracking(purchaseOrder.SupplierId, cancellationToken);

            if (supplier is null)
                return Result<bool>.FailureWithErrorType(
                    $"Supplier record associated with {purchaseOrder.PurchaseOrderCode} could not found",
                    ErrorType.NotFound
                    );

            var requestClotheIds = request.LineItems.Select(x => x.ClotheId).ToList();
            var requestedClothes = await _clotheRepository.GetRangeOfClothesByIdsAsync(requestClotheIds, cancellationToken);

            if(requestedClothes.Count != requestClotheIds.Count)
            {
                var foundClothes = requestedClothes.Select(x => x.Id).ToList();
                var missingClothes = foundClothes.Where(x => !requestClotheIds.Contains(x));

                return Result<bool>.FailureWithErrorType(
                    $"The following clothe identifiers does not exist: {string.Join(", ", missingClothes)}",
                    ErrorType.NotFound
                    );
            }

            var invalidSupplierClothes = requestedClothes.Where(c => c.SupplierId != supplier.Id).ToList();

            if(invalidSupplierClothes.Any())
            {
                var clotheNames = string.Join(", ", invalidSupplierClothes.Select(x => x.ClotheName));
                var supplierName = supplier.SupplierName;
                return Result<bool>.FailureWithErrorType(
                    $"The following clothes doesn't belong to supplier {supplierName}: {clotheNames}",
                    ErrorType.BadRequest
                    );
            }

            var oldPODetails = await _purchaseOrderRepository.GetPurchaseOrderByIdNoTrackingAsync(request.PurchaseOrderId, cancellationToken);

            var requestedItems = _mapper.Map<List<PurchaseOrderItem>>(request.LineItems);
            var alreadyAssignedItems = purchaseOrder.PurchaseOrderItems;

            var newlyAssignedItems = requestedItems.Where(x => !alreadyAssignedItems.Any(i => i.ClotheId == x.ClotheId))
                .ToList();

            var itemsToRemove = alreadyAssignedItems.Where(x => !requestedItems.Any(i => i.ClotheId == x.ClotheId)).ToList();

            var itemsToUpdate = alreadyAssignedItems.Where(x => requestedItems.Any(i => i.ClotheId == x.ClotheId)).ToList();

            foreach (var itemToRemove in itemsToRemove)
                purchaseOrder.PurchaseOrderItems.Remove(itemToRemove);

            foreach (var itemToUpdate in itemsToUpdate)
            {
                var matchingItem = requestedItems.First(x => x.ClotheId == itemToUpdate.ClotheId);
                itemToUpdate.OrderedQuantity = matchingItem.OrderedQuantity;
            }

            foreach(var newItem in newlyAssignedItems)
            {
                newItem.PurchaseOrderId = purchaseOrder.Id;
                newItem.OriginalSupplierId = supplier.Id;
                newItem.UnitCost = requestedClothes.First(x => x.Id == newItem.ClotheId).UnitCost;
                purchaseOrder.PurchaseOrderItems.Add(newItem);
            }

            try
            {
                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var updatePurchaseOrder = await _purchaseOrderRepository.UpdatePurchaseOrderAsync(purchaseOrder, cancellationToken);

                if (!updatePurchaseOrder)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType(
                        "Failed to update purchase order. No changes were saved",
                        ErrorType.BadRequest
                        );
                }

                var auditLog = await _auditLogService.UpdateAuditLogAsync(
                    oldPODetails,
                    purchaseOrder,
                    employee.Id,
                    employee.User.Person.FullName,
                    purchaseOrder.PurchaseOrderCode
                    );

                if(!auditLog)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType(
                        "Failed to create audit log for this action. No changes were saved",
                        ErrorType.BadRequest
                        );
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Purchase order successfully updated!");
            }catch(Exception)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.FailureWithErrorType(
                    "An unexpected error occured",
                    ErrorType.InternalServerError
                    );
            }
        }
    }
}
