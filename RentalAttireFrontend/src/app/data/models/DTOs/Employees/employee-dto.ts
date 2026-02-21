import { PersonDTO } from "../Persons/person-dto";

export class EmployeeDTO {
    id: number = 0;
    employeeCode: string = '';
    department: string = '';
    salary: number = 0;
    rolePosition: string = '';
    person: PersonDTO = new PersonDTO();
}