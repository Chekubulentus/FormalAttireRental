import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Result } from '../../../../data/models/Results/result';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { SupplierDTO } from '../../../../data/models/DTOs/Supplier/supplier';
import { extractErrorMessage } from '../../../../data/utils/error-response-util';
import { filter, firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {
  private baseUrl = `${BaseApiUrl}/Supplier`;

  constructor(
    private httpClient : HttpClient
  ) { }

  async filterSuppliersAsync(
    currentPage : number,
    itemsPerPage : number,
    searchQuery : string = ''
  ) : Promise<Result<PagedResult<SupplierDTO>>> {
    try {
      const filters = {
        searchQuery,
        currentPage,
        itemsPerPage
      };

      var result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<SupplierDTO>>>(
          `${this.baseUrl}/filter-suppliers`,
          { params : filters}
        )
      );

      return result
    }catch(err : any) {
      return Result.failure(extractErrorMessage(err.error));
    }
  }


}
