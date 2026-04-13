using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices;

namespace RentalAttireBackend.Application.Clothes.Commands.ArchiveClothe
{
    public class ArchiveClotheCommandHandler : IRequestHandler<ArchiveClotheCommand, Result<bool>>
    {
        private readonly IClotheRepository _clotheRepo;
        private readonly ITransactionManager _transaction;

        public ArchiveClotheCommandHandler(
            IClotheRepository clotheRepo,
            ITransactionManager transaction            
            )
        {
            _clotheRepo = clotheRepo;
            _transaction = transaction;
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

                clothe.IsActive = false;

                var updateClothe = await _clotheRepo.UpdateClotheAsync(clothe, cancellationToken);

                if(!updateClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe cannot be archived.");
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
