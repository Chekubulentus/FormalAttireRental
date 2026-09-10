import { ClotheDTO } from "../../../../data/models/DTOs/Clothes/clothes";

export interface AssignSupplierClothesModalResponse {
    clothes : ClotheDTO[];
    totalCount : number;
    assignedClothesCount : number;
    unassignedClothesCount : number;
    currentPage : number;
    itemsPerPage : number;
}