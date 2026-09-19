import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Result } from '../../../../data/models/Results/result';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { PurchaseOrderDTO } from '../dtos/purchase-order-dto';
import { BaseApiUrl } from '../../../../../environments/base-api-url';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private baseUrl = `${BaseApiUrl}/PurchaseOrder`;

  constructor(private http: HttpClient) {}

  async filterPurchaseOrdersAsync(
    searchQuery: string,
    statuses: string[],
    dateTypeToggle: 'OrderDate' | 'ExpectedDeliveryDate',
    startingDate: Date | null,
    endingDate: Date | null,
    currentPage: number,
    itemsPerPage: number
  ): Promise<Result<PagedResult<PurchaseOrderDTO>>> {
    try {
      const params = new URLSearchParams();
      if (searchQuery) params.set('searchQuery', searchQuery);
      statuses.forEach(s => params.append('statuses', s)); // repeated key, per [FromQuery] List<string> binding
      params.set('dateTypeToggle', dateTypeToggle);
      if (startingDate) params.set('startingDate', startingDate.toISOString());
      if (endingDate) params.set('endingDate', endingDate.toISOString());
      params.set('currentPage', currentPage.toString());
      params.set('itemsPerPage', itemsPerPage.toString());

      var result =  await firstValueFrom(
        this.http.get<Result<PagedResult<PurchaseOrderDTO>>>(
          `${this.baseUrl}/filter-purchase-orders?${params.toString()}`
        )
      );

      return result;
    } catch (err: any) {
      // ASSUMPTION: Result.failure(message) static factory exists, per project's stated Angular error pattern
      return Result.failure(err?.error?.errorMessage ?? 'Failed to load purchase orders.');
    }
  }
}