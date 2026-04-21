namespace RentalAttireBackend.Application.AuditLogs.DTOs
{
    public class AuditLogResponse
    {
        public List<AuditLogDTO> Logs { get; set; } = new();
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalCount { get; set; }
        public int LoginCount { get; set; }
        public int CreateCount { get; set; }
        public int UpdateCount { get; set; }
        public int ArchiveCount { get; set; }
        public int RestoreCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)ItemsPerPage);
    }
}
