import { ClotheDTO } from "../Clothes/clothes";

export interface SupplierDTO {
    id : number;
    supplierCode : string;
    supplierName : string;
    phoneNumber : string;
    email : string;
    address : string;
    clothesAvailable : ClotheDTO[];
    createdByEmployee : string;
}