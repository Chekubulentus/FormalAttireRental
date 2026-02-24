import { PersonDTO } from "../Persons/person-dto";
import { EmployeeDTO } from "./employee-dto";

export class CreateEmployeeCommand {
    id: number = 0;
    employeeCode: string = '';
    department: string = '';
    salary: number = 0;
    rolePosition: string = '';
    email: string = '';
    password: string = '';
    createdBy: string = '';
    createdById: number = 0;
    person: PersonDTO = new PersonDTO();
}