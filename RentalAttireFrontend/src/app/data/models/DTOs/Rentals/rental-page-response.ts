import { RentalDTO } from './rental';
import { RentalStatus } from './rental-status';

export interface RentalPageResponse {
  items: RentalDTO[];
  currentPage: number;
  itemsPerPage: number;
  totalCount: number;
  totalRevenue: number;
  // Computed over all active rentals, independent of the current search/filter/page
  statusCounts: Partial<Record<RentalStatus, number>>;
  overdueCount: number;
  dueSoonCount: number;
}