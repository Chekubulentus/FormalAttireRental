using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;
using System.Runtime.InteropServices.Marshalling;

namespace RentalAttireBackend.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<bool>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ITransactionManager _transaction;
        private readonly IAuditLogService _auditService;

        public CreateCategoryCommandHandler(
            IMapper mapper,
            ICategoryRepository categoryRepo,
            ITransactionManager transaction,
            IAuditLogService auditService
            )
        {
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _transaction = transaction;
            _auditService = auditService;
        }

        public async Task<Result<bool>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            if (request.PerformedBy is null || request.PerformedById == 0)
                return Result<bool>.Failure("Employee responsible for this transaction could not be identified.");

            try
            {
                var duplicateValidation = await _categoryRepo.CategoryDuplicateValidationAsync(
                    request.CategoryCode,
                    request.CategoryName,
                    cancellationToken
                    );

                if (duplicateValidation)
                    return Result<bool>.Failure("Category already exist.");

                var newCategory = _mapper.Map<Category>(request);

                var createCategory = await _categoryRepo.CreateCategoryAsync(newCategory, cancellationToken);

                if(!createCategory)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Category cannot be created.");
                }

                var auditLog = await _auditService.CreateAuditLogAsync(
                    newCategory,
                    request.PerformedById,
                    request.PerformedBy,
                    request.CategoryName
                    );

                if(!auditLog)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Transaction cannot be audited.");
                }

                return Result<bool>.SuccessWithMessage("Category successfully created.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
