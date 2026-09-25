export interface EditPurchaseOrderCommand {
  purchaseOrderId: number;
  expectedDeliveryDate: string;
  orderStatus: string;
  lineItems: {
    clotheId: number;
    quantity: number;
    unitCost: number;
  }[];
}