using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace RentalAttireBackend.Application.Clothes.Commands.UpdateClothe
{
    public class UpdateClotheCommandHandler : IRequestHandler<UpdateClotheCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IClotheRepository _clotheRepo;
        private readonly ITransactionManager _transaction;
        private readonly IAuditLogService _auditService;

        public UpdateClotheCommandHandler(
            IMapper mapper,
            IClotheRepository clotheRepo,
            ITransactionManager transaction,
            IAuditLogService auditService
            )
        {
            _clotheRepo = clotheRepo;
            _mapper = mapper;
            _transaction = transaction;
            _auditService = auditService;
        }

        public async Task<Result<bool>> Handle(UpdateClotheCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            if (request.PerformedBy is null ||
                request.PerformedById == 0)
                return Result<bool>.Failure("Employee that creates the transaction cannot be audited.");

            try
            {
                var clotheToUpdate = await _clotheRepo.GetClotheByIdAsync(request.Id, cancellationToken);

                if (clotheToUpdate is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe does not exist.");
                }

                var oldClotheDetails = await _clotheRepo.GetClotheByIdNoTrackingAsync(request.Id, cancellationToken);

                _mapper.Map<Clothe>(request);

                var updateClothe = await _clotheRepo.UpdateClotheAsync(clotheToUpdate, cancellationToken);

                if(!updateClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe cannot be updated.");
                }

                var logClotheUpdate = await _auditService.UpdateAuditLogAsync(
                    oldClotheDetails,
                    clotheToUpdate,
                    request.PerformedById,
                    request.PerformedBy,
                    clotheToUpdate.ClotheName
                    );

                if (!logClotheUpdate)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Transaction cannot be audited.");
                }

                return Result<bool>.SuccessWithMessage("Clothe successfully updated.");
            }
            catch(Exception e) {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
