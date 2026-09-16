using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using RentalAttireBackend.Application.Clothes.Helper;

namespace RentalAttireBackend.Application.Clothes.Commands.CreateClothe
{
    public class CreateClotheCommandHandler : IRequestHandler<CreateClotheCommand, Result<bool>>
    {
        private readonly IClotheRepository _clotheRepo;
        private readonly IMapper _mapper;
        private readonly ITransactionManager _transaction;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IAuditLogService _auditService;
        private readonly IFileUploadService _fileUpload;

        public CreateClotheCommandHandler(
            IClotheRepository clotheRepo,
            IMapper mapper,
            ITransactionManager transaction,
            ICategoryRepository categoryRepo,
            IAuditLogService auditService,
            IFileUploadService fileUpload
            )
        {
            _clotheRepo = clotheRepo;
            _mapper = mapper;
            _transaction = transaction;
            _categoryRepo = categoryRepo;
            _auditService = auditService;
            _fileUpload = fileUpload;
        }
        public async Task<Result<bool>> Handle(CreateClotheCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
                return Result<bool>.Failure("Invalid request. Please try again.");

            if (command.PerformedBy is null || 
                command.PerformedById == 0)
                return Result<bool>.Failure("Employee associated with this transaction could not be found.");

            if (command.Image is null)
                return Result<bool>.Failure("Image of the clothe is required.");

            try
            {
                var category = await _categoryRepo.GetCategoryByNameAsync(command.CategoryName, cancellationToken);

                if (category is null)
                    return Result<bool>.FailureWithErrorType("Category is required.", ErrorType.BadRequest);

                var duplicationValidation = await _clotheRepo.ClotheDuplicationValidationAsync(
                    command.ClotheName,
                    cancellationToken
                    );

                if (duplicationValidation)
                    return Result<bool>.FailureWithErrorType("Clothe record already exist.", ErrorType.BadRequest);

                await _transaction.BeginTransactionAsync(cancellationToken);

                var clothe = _mapper.Map<Clothe>(command);
                clothe.CategoryId = category.Id;
                clothe.SupplierId = null;

                var createClothe = await _clotheRepo.CreateClotheAsync(clothe, cancellationToken);

                if (createClothe == 0)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Failed to create clothe record.", ErrorType.BadRequest);
                }

                clothe.ClotheCode = ClotheCodeGenerator.Generate(category.CategoryName, clothe.Id);

                var uploadImage = await _fileUpload.UploadImageAsync(command.Image, $"clothes/{createClothe}");

                if(!uploadImage.Success)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Image cannot be uploaded.", ErrorType.BadRequest);
                }

                clothe.ProfileImagePath = uploadImage.FilePath;
                var updateClothe = await _clotheRepo.UpdateClotheAsync(clothe, cancellationToken);

                if (!updateClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Failed to update clothe. No changes were saved.", ErrorType.BadRequest);
                }

                var auditClothe = await _auditService.CreateAuditLogAsync(
                    clothe,
                    command.PerformedById,
                    command.PerformedBy,
                    clothe.ClotheName
                    );

                if (!auditClothe)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.FailureWithErrorType("Clothe cannot be audited.", ErrorType.BadRequest);
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Clothe successfully created.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
