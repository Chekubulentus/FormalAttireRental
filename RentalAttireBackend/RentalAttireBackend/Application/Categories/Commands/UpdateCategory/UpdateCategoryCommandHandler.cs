using AutoMapper;
using MediatR;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
    {
        private readonly ITransactionManager _transaction;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditService;

        public UpdateCategoryCommandHandler(
            ITransactionManager transaction,
            ICategoryRepository categoryRepo,
            IMapper mapper,
            IAuditLogService auditService
            )
        {
            _transaction = transaction;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.PerformedBy is null ||
               request.PerformedById == 0)
                return Result<bool>.Failure("Employee responsible for this transaction could not be identified.");

            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);

                var category = await _categoryRepo.GetCategoryByIdAsync(request.Id, cancellationToken);

                if (category is null)
                    return Result<bool>.Failure("Category does not exist.");

                var oldCategoryDetails = await _categoryRepo.GetCategoryByIdNoTrackingAsync(request.Id, cancellationToken);

                _mapper.Map(request, category);

                var updateCategory = await _categoryRepo.UpdateCategoryAsync(category, cancellationToken);

                if(!updateCategory)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Category cannot be updated.");
                }

                var auditTransaction = await _auditService.UpdateAuditLogAsync(
                    oldCategoryDetails,
                    category,
                    request.PerformedById,
                    request.PerformedBy,
                    category.CategoryName
                    );

                if (!auditTransaction)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Transaction could not be audited.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Category successfully updated.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
