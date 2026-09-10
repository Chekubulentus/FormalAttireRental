export interface SupplierSummaryDTO {
    id : number;
    supplierCode : string;
    supplierName : string;
    phoneNumber : string;
    email : string;
    address : string;
    assignedClothesCount : number;
    createdByEmployee : string;
    createdAt : Date;
    activePOCount : number;
    overduePOCount : number;
}