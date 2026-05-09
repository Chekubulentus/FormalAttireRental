using AutoMapper;
using RentalAttireBackend.Application.Clothes.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Application.Disposables.DTOs;
using RentalAttireBackend.Domain.Interfaces;

namespace RentalAttireBackend.Application.Disposables.ViewArchivedRecords.ViewClotheRecord
{
    public class ViewClotheRecord : IViewArchivedEntity
    {
        public string EntityType => "Clothe";
        private readonly IClotheRepository _repo;
        private readonly IMapper _mapper;

        public ViewClotheRecord(
            IClotheRepository repo,
            IMapper mapper
            )
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Result<ViewRecordResponse>> GetArchivedRecordAsync(int id, string entityType, CancellationToken ct)
        {
            var clothe = await _repo.GetClotheByIdAsync(id, ct);

            if (clothe is null)
                return Result<ViewRecordResponse>.Failure("Record does not exist.");

            var clotheDto = _mapper.Map<ClotheDTO>(clothe);

            return Result<ViewRecordResponse>.Success(new ViewRecordResponse
            {
                EntityType = "Clothe",
                Record = clotheDto
            });
        }
    }
}
