import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { Result } from '../../../../data/models/Results/result';
import { AuthenticationResult } from '../../../../data/models/Results/authentication-result';
import { firstValueFrom } from 'rxjs';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { Customer } from '../../../../data/models/DTOs/Customer/customer';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private customerUrl = `${BaseApiUrl}/Customer`;

  constructor(
    private httpClient : HttpClient
  ) { }

  async filterClothesAsync(
    currentPage : number,
    itemsPerPage : number,
    searchQuery : string
  ) : Promise<Result<PagedResult<Customer>>> {
    try {
      var params =  {
        currentPage,
        itemsPerPage,
        searchQuery
      };

      var result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<Customer>>>(
          `${this.customerUrl}/filter-customers`, {params: params}
        )
      );

      return result;
    }catch(err : any) {
      return Result.failure(err.error);
    }
  }
}
