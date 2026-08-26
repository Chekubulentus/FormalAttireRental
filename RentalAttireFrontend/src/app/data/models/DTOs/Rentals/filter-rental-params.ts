export interface FilterRentalsParams {
  status?: string;
  searchQuery?: string;
  currentPage: number;
  itemsPerPage: number;
  startingDate?: string;
  endingDate?: string;
}