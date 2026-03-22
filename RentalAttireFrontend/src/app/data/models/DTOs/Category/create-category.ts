import { Category } from "./category";

export class CreateCategoryCommand extends Category{
    performedBy : string = '';
    performedById: number = 0;
}