import { SupplierDTO } from "../Supplier/supplier";

export class ClotheDTO {
  id : number = 0;
  clotheCode: string = '';
  clotheName: string = '';
  categoryName: string = '';
  color: string = '';
  brand: string = '';
  material: string = '';
  size: string = '';
  clotheGender: string = '';
  stockQuantity: number = 0;
  availableQuantity: number = 0;
  rentalPrice: number = 0;
  depositAmount: number = 0;
  rentalDurationDays: number = 0;
  condition: string = '';
  reservedQuantity: number = 0;
  isAvailable: boolean = false;
  rentalCount: number = 0;
  unitCost: number = 0;
  profileImagePath?: string = '';
  supplier : SupplierDTO | null = null;
}