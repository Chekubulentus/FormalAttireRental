export interface UpdateSupplierCommand {
    supplierId : number;
    supplierName : string;
    phoneNumber ?: string;
    email ?: string;
    address : string;
    assignClotheIds : number[];
    unassignClotheIds : number[]; 
}