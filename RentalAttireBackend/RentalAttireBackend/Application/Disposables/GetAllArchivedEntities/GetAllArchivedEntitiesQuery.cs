using MediatR;
using RentalAttireBackend.Application.Categories.DTOs;
using RentalAttireBackend.Application.Common.Models;

namespace RentalAttireBackend.Application.Disposables.GetAllArchivedEntities
{
    public class GetAllArchivedEntitiesQuery : IRequest<Result<PagedResult<ArchivedEntityDto>>>
    {
        public PaginationParams PaginationParams { get; set; } = new();
    }
}
