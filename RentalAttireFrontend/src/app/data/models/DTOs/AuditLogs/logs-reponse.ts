import { AuditLog } from "./audit-log";

export class LogsReponse{
    logs: AuditLog[] = [];
    currentPage: number = 0;
    itemsPerPage: number = 0;
    totalPages: number = 0;
    totalCount: number = 0;
    loginCount: number = 0;
    createCount: number = 0;
    updateCount: number = 0;
    archiveCount: number = 0;
    restoreCount : number = 0;
}