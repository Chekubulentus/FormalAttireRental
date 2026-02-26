import { EmployeeDTO } from "./employee-dto";

export class UpdateEmployeeCommand extends EmployeeDTO{
    performedBy: string = '';
    performedById: number = 0;
}