import { ClotheDTO } from "../../../../data/models/DTOs/Clothes/clothes";

export interface CreatePurchaseOrderCommand {
    lineItems : LineItem[];
    supplierId : number;
    expectedDeliveryDate : Date;
    orderStatus : string;
}

interface LineItem {
  clotheId : number;
  quantity: number;
  unitCost: number;
}