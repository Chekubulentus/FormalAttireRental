import { Category } from "./category";

export class UpdateCategoryCommand extends Category{ 
    performedBy : string = '';
    performedById : number = 0;
}