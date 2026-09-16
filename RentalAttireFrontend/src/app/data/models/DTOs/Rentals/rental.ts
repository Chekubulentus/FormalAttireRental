import { RentalItemDTO } from "./rental-item";
import { RentalStatus } from "./rental-status";
import { Customer } from "../Customer/customer";

export interface RentalDTO {
  id: number;
  rentalCode: string;
  customer : Customer;
  pickupDate: string;
  returnDate: string;
  totalAmount : number;
  status: RentalStatus;
  depositAmount: number;
  paymentMethod: 'Cash' | 'Gcash';
  rentalItems : RentalItemDTO[];
  gcashReferenceNumber: string;
  gcashReferenceName: string;
}