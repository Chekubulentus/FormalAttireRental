import { RentalItemRequest } from "./rental-item-request";

export interface RentalTransactionRequest {
    pickupDate: Date;
    returnDate: Date;
    paymentMethod: string;
    gcashRefNum: string;
    gcashRefName: string;
    rentalItems: RentalItemRequest[];
}