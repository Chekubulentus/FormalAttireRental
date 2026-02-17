export class PagedResult<T> {
    items : T[] = [];
    totalCount : number = 0;
    pageNumber : number = 0;
    pageSize : number = 0;
    totalPages : number = 0;
}