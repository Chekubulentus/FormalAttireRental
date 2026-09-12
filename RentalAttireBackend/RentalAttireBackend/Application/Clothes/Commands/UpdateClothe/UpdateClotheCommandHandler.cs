using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Clothes.Helper;
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
        private readonly IFileUploadService _fileUploadService;
        private readonly ICategoryRepository _categoryRepo;

        public UpdateClotheCommandHandler(
            IMapper mapper,
            IClotheRepository clotheRepo,
            ITransactionManager transaction,
            IAuditLogService auditService,
            IFileUploadService fileUploadService,
            ICategoryRepository categoryRepo
            )
        {
            _clotheRepo = clotheRepo;
            _mapper = mapper;
            _transaction = transaction;
            _auditService = auditService;
            _fileUploadService = fileUploadService;
            _categoryRepo = categoryRepo;
        }

        public async Task<Result<bool>> Handle(UpdateClotheCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            if (request.PerformedBy is null ||
                request.PerformedById == 0)
                return Result<bool>.Failure("Employee associated with this transaction could not be audited.");

            try
            {
                var category = await _categoryRepo.GetCategoryByNameAsync(request.CategoryName, cancellationToken);

                if (category is null)
                    return Result<bool>.FailureWithErrorType($"Category is required.", ErrorType.NotFound);

                var clotheToUpdate = await _clotheRepo.GetClotheByIdAsync(request.Id, cancellationToken);

                if (clotheToUpdate is null)
                    return Result<bool>.FailureWithErrorType("Clothe does not exist.", ErrorType.NotFound);

                var oldClotheDetails = await _clotheRepo.GetClotheByIdNoTrackingAsync(request.Id, cancellationToken);

                clotheToUpdate.CategoryId = category.Id;

                await _transaction.BeginTransactionAsync(cancellationToken);

                if (request.Image is not null)
                {
                    await _fileUploadService.DeleteFileAsync(clotheToUpdate.ProfileImagePath);

                    var uploadImage = await _fileUploadService.UploadImageAsync(request.Image, $"clothes/{clotheToUpdate.Id}");

                    if (!uploadImage.Success)
                    {
                        await _transaction.RollbackTransactionAsync(cancellationToken);
                        return Result<bool>.Failure("Image could not be uploaded.");
                    }

                    clotheToUpdate.ProfileImagePath = uploadImage.FilePath;
                }

                _mapper.Map(request, clotheToUpdate);
                clotheToUpdate.ClotheCode = ClotheCodeGenerator.Generate(category.CategoryName, clotheToUpdate.Id);

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

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Clothe successfully updated.");
            }
            catch(Exception e) {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
