import { PersonDTO } from "../Persons/person-dto";

export class RegisterCustomerCommand {
    password : string = '';
    confirmPassword : string = '';
    id: number = 0; 
    customerCode : string = '';
    totalRentals : number = 0;
    email: string = '';
    isGoogleAccount : boolean = false;
    person : PersonDTO = new PersonDTO();
}