using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Common;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IAuditLogService
    {
        public Task<bool> CreateAuditLogAsync<T>(T entity, int personId, string personName) where T : BaseEntity;
        public Task<bool> UpdateAuditLogAsync<T>(T oldEntity, T newEntity, int personId, string personName) where T : BaseEntity;
        public Task<bool> ArchiveAuditLogAsync<T>(T oldEntity, T newEntity, int personId, string personName) where T : BaseEntity;
        public Task<PagedResult<AuditLog>> GetAllAuditLogsAsync(PaginationParams paginationParams);
        public Task<bool> LoginAuditLogAsync<T>(T entity) where T : BaseEntity;
        public Task<bool> ViewAuditLogAsync<T>(T viewer, T target, int personId, string personName) where T : BaseEntity;

    }
}
