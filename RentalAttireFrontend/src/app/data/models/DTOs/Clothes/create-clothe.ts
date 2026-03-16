import { ClotheDTO } from "./clothes";

export class CreateClotheCommand extends ClotheDTO{
    performedBy: string = '';
    performedById: number = 0;
    image: File | null = null;
}