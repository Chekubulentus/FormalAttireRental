import { PersonDTO } from "../Persons/person-dto";

export class Customer {
    id : number = 0;
    customerCode : string = '';
    totalRentals : number = 0;
    totalSpent : number = 0;
    email : string = '';
    isGoogleAccount : boolean = true;
    person : PersonDTO = new PersonDTO();
}