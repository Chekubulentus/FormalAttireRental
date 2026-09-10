export interface AssignClothesModalQuery  {
    supplierId ?: number;
    searchQuery ?: string;
    category ?: string;
    gender ?: string;
    currentPage : number;
    itemsPerPage : number;
}