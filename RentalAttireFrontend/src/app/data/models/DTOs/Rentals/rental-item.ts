import { ClotheDTO } from "../Clothes/clothes";

export interface RentalItemDTO {
  id: number;
  rentalCode : string;
  clothe: ClotheDTO;
  rentalPrice : number;
  quantity: number;
  totalAmount: number;
}