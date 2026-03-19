using MediatR;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Categories.Commands.ArchiveCategory
{
    public class ArchiveCategoryByIdCommandHandler : IRequestHandler<ArchiveCategoryByIdCommand, Result<bool>>
    {
        private readonly ITransactionManager _transaction;
        private readonly ICategoryRepository _categoryRepo;

        public ArchiveCategoryByIdCommandHandler(
            ICategoryRepository categoryRepo,
            ITransactionManager transaction
            )
        {
            _categoryRepo = categoryRepo;
            _transaction = transaction;
        }

        public async Task<Result<bool>> Handle(ArchiveCategoryByIdCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<bool>.Failure("Invalid request.");

            if (request.PerformedBy is null || request.PerformedById == 0)
                return Result<bool>.Failure("Employee responsible for this transaction could not be identified.");
            
            try
            {
                await _transaction.BeginTransactionAsync(cancellationToken);

                var category = await _categoryRepo.GetCategoryByIdAsync(request.Id, cancellationToken);

                if(category is null)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Category does not exist.");
                }

                category.IsActive = false;

                var updateCategory = await _categoryRepo.UpdateCategoryAsync(category, cancellationToken);

                if(!updateCategory)
                {
                    await _transaction.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Category could not be archived.");
                }

                await _transaction.CommitTransacionAsync(cancellationToken);
                return Result<bool>.SuccessWithMessage("Category successfully archived.");
            }catch(Exception e)
            {
                await _transaction.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(e.Message);
            }
        }
    }
}
