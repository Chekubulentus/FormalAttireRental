using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace RentalAttireBackend.Application.Clothes.Commands.ArchiveClothe
{
    public class ArchiveClotheCommandHandler : IRequestHandler<ArchiveClotheCommand, Result<bool>>
    {
        private readonly IClotheRepository _clotheRepo;
        private readonly ITransactionManager _transaction;
        private readonly IAuditLogService _auditService;

        public ArchiveClotheCommandHandler(
            IClotheRepository clotheRepo,
            ITransactionManager transaction,
            IAuditLogService auditService
            )
        {
            _clotheRepo = clotheRepo;
            _transaction = transaction;
            _auditService = auditService;
        }

        public async Task<Result<bool>> Handle(ArchiveClotheCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
                return Result<bool>.Failure("Invalid request.");

            if (request.PerformedBy is null ||
                request.PerformedById == 0)
                return Result<bool>.Failure("Employee associated with this transcation could not be found.");
            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);

                var clothe = await _clotheRepo.GetClotheByIdAsync(request.Id, cancellationToken);

                if(clothe is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe does not exist.");
                }

                if (clothe.StockQuantity > 0)
                    return Result<bool>.Failure("Clothe still has some available stocks.");

                clothe.IsActive = false;
                clothe.ArchivedAt = DateTime.UtcNow;
                clothe.ArchivedBy = request.PerformedBy;

                var updateClothe = await _clotheRepo.UpdateClotheAsync(clothe, cancellationToken);

                if(!updateClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe cannot be archived.");
                }

                var auditTransaction = await _auditService.ArchiveAuditLogAsync(
                    clothe,
                    request.PerformedById,
                    request.PerformedBy,
                    clothe.ClotheName
                    );

                if(!auditTransaction)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Failed to record audit log for this action. No changes were saved.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Clothe successfully archived.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
