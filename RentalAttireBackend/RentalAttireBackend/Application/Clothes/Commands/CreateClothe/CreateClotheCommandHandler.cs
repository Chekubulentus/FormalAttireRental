using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Clothes.Commands.CreateClothe
{
    public class CreateClotheCommandHandler : IRequestHandler<CreateClotheCommand, Result<bool>>
    {
        private readonly IClotheRepository _clotheRepo;
        private readonly IMapper _mapper;
        private readonly ITransactionManager _transaction;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IAuditLogService _auditService;

        public CreateClotheCommandHandler(
            IClotheRepository clotheRepo,
            IMapper mapper,
            ITransactionManager transaction,
            ICategoryRepository categoryRepo,
            IAuditLogService auditService
            )
        {
            _clotheRepo = clotheRepo;
            _mapper = mapper;
            _transaction = transaction;
            _categoryRepo = categoryRepo;
            _auditService = auditService;
        }
        public async Task<Result<bool>> Handle(CreateClotheCommand command, CancellationToken cancellationToken)
        {
            if (command.PerfomedBy is null || 
                command.PerformedById == 0)
                return Result<bool>.Failure("Employee that creates the transaction cannot be audited.");

            if (command.Image is null)
                return Result<bool>.Failure("Image of the clothe is required.");

            if (command is null)
                return Result<bool>.Failure("Invalid request. Please try again.");

            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);

                var category = await _categoryRepo.GetCategoryByNameAsync(command.CategoryName, cancellationToken);

                if (category is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Category is required.");
                }

                var clothe = _mapper.Map<Clothe>(command);

                var auditClothe = await _auditService.CreateAuditLogAsync(
                    clothe,
                    command.PerformedById,
                    command.PerfomedBy,
                    clothe.ClotheName
                    );

                if(!auditClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe cannot be audited.");
                }

                var createClothe = await _clotheRepo.CreateClotheAsync(clothe, cancellationToken);

                if(!createClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe cannot be created.");
                }

                return Result<bool>.SuccessWithMessage("Clothe successfully created.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
