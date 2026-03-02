using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Common;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IAuditLogService
    {
        public Task<bool> CreateAuditLogAsync<T>(T entity, int personId, string personName) where T : BaseEntity;
        public Task<bool> UpdateAuditLogAsync<T>(T oldEntity, T newEntity, int personId, string personName) where T : BaseEntity;
        public Task<bool> ArchiveAuditLogAsync<T>(T entity, int personId, string personName) where T : BaseEntity;
        public Task<AuditLogResponse> GetAllAuditLogsAsync(
            string actionType,
            string? searchQuery,
            int currentPage,
            int itemsPerPage,
            DateTime? dateFrom,
            DateTime? dateTo,
            CancellationToken cancellationToken
            );
        public Task<bool> LoginAuditLogAsync<T>(T entity) where T : BaseEntity;
        public Task<bool> ViewAuditLogAsync<T>(T viewer, T target, int personId, string personName) where T : BaseEntity;

    }
}
