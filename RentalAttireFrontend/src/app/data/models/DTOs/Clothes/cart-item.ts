import { ClotheDTO } from "./clothes";

export interface CartItem {
  clothe: ClotheDTO;
  quantity: number;
}