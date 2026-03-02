export class AuditLog {
    id: number = 0;
    entityType: string = '';
    entityName: string = '';
    actionType: string = '';
    changedBy: string = '';
    changedAt: Date = new Date();
    ipAddress: string = '';
}