import { PurchaseOrderItemDTO } from "./purchase-order-item-dto";

export interface PurchaseOrderDTO {
    id : number;
    purchaseOrderCode : string;
    orderDate : Date;
    expectedDeliveryDate : Date;
    supplierName : string;
    orderStatus : string;
    employeeName : string;
    totalAmount : number;
    purchaseOrderItems : PurchaseOrderItemDTO[];
}