using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Common;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Application.Common.Interfaces
{
    public interface IAuditLogService
    {
        public Task<bool> CreateAuditLogAsync<T>(
            T entity, 
            int personId, 
            string personName, 
            string entityNameCreated) where T : BaseEntity;
        public Task<bool> UpdateAuditLogAsync<T>(
            T oldEntity, 
            T newEntity, 
            int personId, 
            string personName, 
            string personUpdated) where T : BaseEntity;
        public Task<bool> ArchiveAuditLogAsync<T>(T entity, int personId, string personName, string updatedName) where T : BaseEntity;
        public Task<AuditLogResponse> GetAllAuditLogsAsync(
            string actionType,
            string? searchQuery,
            int currentPage,
            int itemsPerPage,
            DateTime? dateFrom,
            DateTime? dateTo,
            CancellationToken cancellationToken
            );
        public Task<bool> LoginAuditLogAsync<T>(T entity, string name, int id) where T : BaseEntity;
        public Task<bool> ViewAuditLogAsync<T>(T viewer, T target, int personId, string personName) where T : BaseEntity;
        public Task<bool> RestorationAuditLogAsync<T>(
            T entity,
            string performedBy,
            int performedById,
            string entityNameRestored
            ) where T : BaseEntity;
        public Task<bool> DeleteAuditLogAsync<T>(
            T entity,
            string performedBy,
            int performedById,
            string recordName
            ) where T : BaseEntity;

        public Task<bool> RentalReservationAuditLogAsync<T>(
            T entity,
            string reservedBy,
            int reserverdById,
            string rentalCode
            ) where T : BaseEntity;

        public Task<bool> UpdateRentalReservationAsync<T>(
            T rental,
            string status,
            string rentalCode,
            string actionType,
            string performedBy,
            int performedById
            ) where T : BaseEntity;
    }
}
