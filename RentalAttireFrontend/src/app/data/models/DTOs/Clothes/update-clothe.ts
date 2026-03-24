import { ClotheDTO } from "./clothes";

export class UpdateClotheCommand extends ClotheDTO {
    performedBy : string = '';
    performedById : number = 0;
    
}