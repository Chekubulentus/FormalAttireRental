using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.PurchaseOrders.Commands.CreatePurchaseOrder
{
    public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<bool>>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IClotheRepository _clotheRepository;
        private readonly ISupplierRepository _supplierRepository;

        public CreatePurchaseOrderCommandHandler(
            ITransactionManager transactionManager,
            IPurchaseOrderRepository purchaseOrderRepository,
            ICurrentUserService currentUserService,
            IAuditLogService auditLogService,
            IMapper mapper,
            IUserRepository userRepository,
            IClotheRepository clotheRepository,
            ISupplierRepository supplierRepository
            )
        {
            _transactionManager = transactionManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _currentUserService = currentUserService;
            _auditLogService = auditLogService;
            _mapper = mapper;
            _userRepository = userRepository;
            _clotheRepository = clotheRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<Result<bool>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result<bool>.FailureWithErrorType("Invalid request", ErrorType.BadRequest);

                var currentUserId = _currentUserService.UserId;

                var currentUser = await _userRepository.GetUserWithEmployeeAsync(currentUserId ?? 0, cancellationToken);

                if (currentUser is null || currentUser.Employee is null)
                    return Result<bool>.FailureWithErrorType("User associated with this action could not be found", ErrorType.NotFound);

                var supplier = await _supplierRepository.GetSupplierByIdAsyncNoTracking(request.SupplierId, cancellationToken);

                if (supplier is null)
                    return Result<bool>.FailureWithErrorType("Supplier record does not exist", ErrorType.NotFound);

                var purchaseOrderItems = _mapper.Map<List<PurchaseOrderItem>>(request.LineItems);
                var clotheIds = purchaseOrderItems.Select(x => x.ClotheId).Distinct().ToList();

                var purchaseOrder = _mapper.Map<PurchaseOrder>(request);
                purchaseOrder.Employee = currentUser.Employee;

                var selectedClothes = await _clotheRepository.GetRangeOfClothesByIdsAsync(clotheIds, cancellationToken);

                if (selectedClothes.Count != clotheIds.Count)
                {
                    var foundIds = selectedClothes.Select(c => c.Id).ToHashSet();
                    var missingIds = clotheIds.Where(id => !foundIds.Contains(id)).ToList();
                    return Result<bool>.FailureWithErrorType(
                        $"The following clothe ids do not exist: {string.Join(", ", missingIds)}",
                        ErrorType.NotFound);
                }

                var invalidClothes = selectedClothes.Where(x => x.SupplierId != supplier.Id).ToList();

                if(invalidClothes.Any())
                {
                    var names = string.Join(", ", invalidClothes.Select(x => x.ClotheName));
                    return Result<bool>.FailureWithErrorType(
                        $"The following clothes are not assigned to {supplier.SupplierName}: {names}",
                        ErrorType.BadRequest
                        );
                }

                foreach(var item in purchaseOrderItems)
                {
                    item.OriginalSupplierId = supplier.Id;
                    item.PurchaseOrder = purchaseOrder;
                }

                purchaseOrder.PurchaseOrderItems = purchaseOrderItems;

                await _transactionManager.BeginTransactionAsync(cancellationToken);

                var createPurchaseOrder = await _purchaseOrderRepository.CreatePurchaseOrderAsync(purchaseOrder, cancellationToken);

                if(!createPurchaseOrder)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType(
                        "Purchase Order could not be created. No changes were saved",
                        ErrorType.BadRequest
                        );
                }

                var auditLog = await _auditLogService.CreateAuditLogAsync(
                    purchaseOrder,
                    currentUser.Employee.Id,
                    currentUser.Employee.User.Person.FullName,
                    purchaseOrder.PurchaseOrderCode
                    );

                if (!auditLog)
                {
                    await _transactionManager.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType(
                        "Failed to create audit log for this action. No changes were saved",
                        ErrorType.BadRequest
                        );
                }

                await _transactionManager.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Purchase Order successfully requested");
            }catch(Exception)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
