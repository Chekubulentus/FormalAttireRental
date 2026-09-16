export interface CreateSupplierCommand {
    supplierName: string;
    phoneNumber ?: string;
    email ?: string;
    address : string;
    clotheIds : number[];
}