import { ClotheDTO } from "../../../../data/models/DTOs/Clothes/clothes";

export interface PurchaseOrderItemDTO {
    id : number;
    purchaseOrderId : number;
    clothe : ClotheDTO;
    orderedQuantity : number;
    receivedQuantity : number;
    unitCost : number;
    originalSupplierId : number;
    totalAmount : number;
}