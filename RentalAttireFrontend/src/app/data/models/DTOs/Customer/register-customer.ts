import { PersonDTO } from "../Persons/person-dto";

export class RegisterCustomerCommand {
    password : string = '';
    confirmPassword : string = '';
    performedBy : string = '';
    performedById : number = 0;
    id: number = 0; 
    customerCode : string = '';
    totalRentals : number = 0;
    totalSpent : number = 0;
    email: string = '';
    isGoogleAccount : boolean = false;
    person : PersonDTO = new PersonDTO();
}