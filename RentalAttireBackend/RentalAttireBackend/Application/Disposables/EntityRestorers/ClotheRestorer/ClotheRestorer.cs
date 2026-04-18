using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.EntityRestorers.ClotheRestorer
{
    public class ClotheRestorer : IEntityRestorer
    {
        private readonly IClotheRepository _repo;
        public string EntityType => "Clothe";

        public ClotheRestorer(IClotheRepository repo) => _repo = repo;

        public async Task<Result<bool>> RestoreAsync(int entityId, CancellationToken ct)
        {
            var clotheToRestore = await _repo.GetClotheByIdAsync(entityId, ct);

            if (clotheToRestore is null)
                return Result<bool>.Failure("Clothe does not exist.");

            clotheToRestore.IsActive = true;

            var updateRecord = await _repo.UpdateClotheAsync(clotheToRestore, ct);

            if (!updateRecord)
                return Result<bool>.Failure("Record could not be restored. Please try again.");

            return Result<bool>.SuccessWithMessage("Record successfully restored.");
        }
    }
}
