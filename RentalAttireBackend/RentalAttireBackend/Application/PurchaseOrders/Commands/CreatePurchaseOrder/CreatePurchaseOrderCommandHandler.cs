using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
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

        public CreatePurchaseOrderCommandHandler(
            ITransactionManager transactionManager,
            IPurchaseOrderRepository purchaseOrderRepository,
            ICurrentUserService currentUserService,
            IAuditLogService auditLogService,
            IMapper mapper
            )
        {
            _transactionManager = transactionManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _currentUserService = currentUserService;
            _auditLogService = auditLogService;
            _mapper = mapper;
        }

        public async Task<Result<bool>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result<bool>.FailureWithErrorType("Invalid request", ErrorType.BadRequest);

                throw new NotImplementedException();
            }catch(Exception)
            {
                await _transactionManager.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
