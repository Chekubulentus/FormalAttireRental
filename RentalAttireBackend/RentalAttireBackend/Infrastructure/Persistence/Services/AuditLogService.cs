using AutoMapper;
using Google.Apis.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RentalAttireBackend.Application.AuditLogs.DTOs;
using RentalAttireBackend.Application.Common.Interfaces;
using RentalAttireBackend.Application.Common.Models;
using RentalAttireBackend.Domain.Common;
using RentalAttireBackend.Domain.Entities;
using RentalAttireBackend.Infrastructure.Persistence.DataContext;
using System.Text.Json;

namespace RentalAttireBackend.Infrastructure.Persistence.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly FormalAttireContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuditLogService
            (
            FormalAttireContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> ArchiveAuditLogAsync<T>(
            T entity, 
            int personId, 
            string personName, 
            string updatedName) where T : BaseEntity
        {
            var newAuditLog = new AuditLog
            {
                EntityType = typeof(T).Name,
                EntityId = entity.Id,
                EntityName = updatedName,
                ActionType = "Archived",
                ChangedBy = personName,
                ChangedById = personId,
                IpAddress = GetIpAddress()
            };

            await _context.AuditLogs.AddAsync(newAuditLog);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateAuditLogAsync<T>(
            T entity,
            int personId, 
            string personName,
            string entityCreated
            ) where T : BaseEntity
        {
            var newAuditLog = new AuditLog
            {
                EntityType = typeof(T).Name,
                EntityId = entity.Id,
                EntityName = entityCreated,
                ActionType = "Create",
                ChangedBy = personName,
                ChangedById = personId,
                OldValues = null,
                NewValues = JsonSerializer.Serialize(entity),
                IpAddress = GetIpAddress()
            };

            await _context.AuditLogs.AddAsync(newAuditLog);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<AuditLogResponse> GetAllAuditLogsAsync(
            string? actionType,
            string? searchQuery,
            int currentPage,
            int itemsPerPage,
            DateTime? dateFrom,
            DateTime? dateTo,
            CancellationToken cancellationToken
            )
        {
            var searchQueryValidator = string.IsNullOrEmpty(searchQuery);
            var actionTypeValidator = string.IsNullOrEmpty(actionType);

            var auditLogs = _context.AuditLogs
                .OrderByDescending(al => al.Id)
                .AsNoTracking()
                .Where(al =>
                (searchQueryValidator ||
                al.ChangedBy.ToLower().Contains(searchQuery.ToLower()) ||
                al.EntityName.ToLower().Contains(searchQuery.ToLower()))
                &&
                (actionTypeValidator || al.ActionType.ToLower().Equals(actionType.ToLower()))
                &&
                (!dateFrom.HasValue || al.ChangedAt >= dateFrom)
                &&
                (!dateTo.HasValue || al.ChangedAt <= dateTo)
                );

            var totalCount = await auditLogs.CountAsync();
            var loginCount = await auditLogs.CountAsync(al => al.ActionType.ToLower().Equals("login"));
            var createCount = await auditLogs.CountAsync(al => al.ActionType.ToLower().Equals("create"));
            var updateCount = await auditLogs.CountAsync(al => al.ActionType.ToLower().Equals("update"));
            var archiveCount = await auditLogs.CountAsync(al => al.ActionType.ToLower().Equals("archived"));

            int skippedItemsCount = (currentPage - 1) * itemsPerPage;

            var items = await auditLogs
                .Skip(skippedItemsCount)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            var itemDtos = _mapper.Map<List<AuditLogDTO>>(items);

            return new AuditLogResponse
            {
                Logs = itemDtos,
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                TotalCount = totalCount,
                LoginCount = loginCount,
                CreateCount = createCount,
                UpdateCount = updateCount,
                ArchiveCount = archiveCount
            };
        }

        public async Task<bool> UpdateAuditLogAsync<T>(
            T oldEntity, 
            T newEntity, 
            int personId, 
            string personName,
            string personUpdated
            ) where T : BaseEntity
        {
            var changes = GetChanges(oldEntity, newEntity);

            if (!changes.Any())
                return false;

            var newAuditLog = new AuditLog
            {
                EntityType = typeof(T).Name,
                EntityId = newEntity.Id,
                EntityName = personUpdated,
                ActionType = "Update",
                ChangedBy = personName,
                ChangedById = personId,
                OldValues = JsonSerializer.Serialize(oldEntity),
                NewValues = JsonSerializer.Serialize(newEntity),
                IpAddress = GetIpAddress()
            };

            await _context.AuditLogs.AddAsync(newAuditLog);
            return await _context.SaveChangesAsync() > 0;
        }

        //Helpers
        private string GetIpAddress()
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            return ipAddress ?? "IP address unknown";
        }
        private Dictionary<string, (object oldValues, object newValues)> GetChanges<T>(T oldEntity, T newEntity)
        {
            var changes = new Dictionary<string, (object oldValues, object newValues)>();
            var properties = typeof(T).GetProperties();

            var ignoredProperties = new HashSet<string>
            {
                "Id",
                "CreatedBy",
                "CreatedAt",
                "UpdatedBy",
                "UpdatedAt",
                "ArchivedBy",
                "ArchivedAt",
                "EntityType",
                "IsDeleted",
                "IsActive",
                "EntityType"
            };

            foreach(var property in properties)
            {
                if (ignoredProperties.Contains(property.Name))
                    continue;

                var oldValues = property.GetValue(oldEntity);
                var newValues = property.GetValue(newEntity);

                if(!Equals(oldValues, newValues))
                {
                    changes[property.Name] = (oldValues, newValues);
                }
            }
            return changes;
        }
        private Dictionary<string, object> GetChangesFromAuditLog(AuditLog auditLog)
        {
            var changes = new Dictionary<string, object>();

            if(auditLog.ActionType == "Create")
            {
                var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLog.NewValues);

                foreach(var property in newValues)
                {
                    changes[property.Key] = new { From = (object)null, To = property.Value };
                }
            }else if(auditLog.ActionType == "Update")
            {
                var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLog.NewValues);
                var oldValues = JsonSerializer.Deserialize<Dictionary<string, object>>(auditLog.OldValues);

                foreach(var property in newValues.Keys)
                {
                    changes[property] = new { From = oldValues[property], To = newValues[property] };
                }
            }

            return changes;
        }

        public async Task<bool> ViewAuditLogAsync<T>(T viewer, T target, int personId, string personName) where T : BaseEntity
        {
            var auditLog = new AuditLog
            {
                EntityType = typeof(T).Name,
                EntityId = viewer.Id,
                ActionType = "Viewed",
                ChangedBy = personName,
                ChangedById = personId,
                IpAddress = GetIpAddress(),
            };

            await _context.AuditLogs.AddAsync(auditLog);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LoginAuditLogAsync<T>(T entity, string name, int id) where T : BaseEntity
        {
            var auditLog = new AuditLog
            {
                EntityType = typeof(T).Name,
                EntityId = entity.Id,
                ActionType = "Login",
                ChangedBy = name,
                ChangedById = id,
                OldValues = null,
                NewValues = null,
                IpAddress = GetIpAddress()
            };

            await _context.AuditLogs.AddAsync(auditLog);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
