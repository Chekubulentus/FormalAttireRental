using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.Versioning;

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
                await _transaction.BeginTransactionAsync(cancellationToken);

                var category = await _categoryRepo.GetCategoryByNameAsync(command.CategoryName, cancellationToken);

                if (category is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Category is required.");
                }

                var clothe = _mapper.Map<Clothe>(command);
                clothe.CategoryId = category.Id;

                var createClothe = await _clotheRepo.CreateClotheAsync(clothe, cancellationToken);

                var uploadImage = await _fileUpload.UploadImageAsync(command.Image, $"clothes/{createClothe}");

                if(!uploadImage.Success)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Image cannot be uploaded.");
                }

                clothe.ProfileImagePath = uploadImage.FilePath;
                await _clotheRepo.UpdateClotheAsync(clothe, cancellationToken);

                if (createClothe == 0)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Clothe cannot be created.");
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
                    return Result<bool>.Failure("Clothe cannot be audited.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Clothe successfully created.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
