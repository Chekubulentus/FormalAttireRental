import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Result } from '../../../../data/models/Results/result';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { SupplierDTO } from '../../../../data/models/DTOs/Supplier/supplier';
import { extractErrorMessage } from '../../../../data/utils/error-response-util';
import { filter, first, firstValueFrom } from 'rxjs';
import { CreateSupplierCommand } from '../dtos/create-supplier-command';
import { UpdateSupplierCommand } from '../dtos/update-supplier-command';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

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
      var params = new HttpParams()
      .set('currentPage', currentPage)
      .set('itemsPerPage', itemsPerPage);

      if(searchQuery) 
        params = params.set('searchQuery', searchQuery);

      var result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<SupplierDTO>>>(
          `${this.baseUrl}/filter-suppliers`,
          { params }
        )
      );

      return result
    }catch(err : any) {
      return Result.failure(extractErrorMessage(err.error));
    }
  }

  async getSupplierByIdAsync(
    id : number
  ) : Promise<Result<SupplierDTO>> {
    try {
      var result = await firstValueFrom(
        this.httpClient.get<Result<SupplierDTO>>(
          `${this.baseUrl}/${id}`
        )
      )      
      return result;
    }catch(err : any) {
      return Result.failure(extractErrorMessage(err.error));
    }
  }

  async createSupplierAsync(
    command : CreateSupplierCommand
  ) : Promise<Result<boolean>>  {
    try {
      const result = await firstValueFrom(
        this.httpClient.post<Result<boolean>>(
          `${this.baseUrl}`,
          command
        )
      )
      return result;
    }catch(err : any) {
      return Result.failure(extractErrorMessage(err.error));
    }
  }

  async updateSupplierAsync(
    command : UpdateSupplierCommand
  ) : Promise<Result<boolean>> {
    try {
      const result = await firstValueFrom(
        this.httpClient.put<Result<boolean>>(
          `${this.baseUrl}`,
          command
        )
      );

      return result;
    }catch(err : any) {
      return Result.failure(extractErrorMessage(err.error));
    }
  }

  async getSupplierClothesByIdAsync(
    id : number,
    currentPage : number,
    itemsPerPage : number
  ) : Promise<Result<PagedResult<ClotheDTO>>> {
    try {
      const filters = {
        id,
        currentPage,
        itemsPerPage
      }

      const result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<ClotheDTO>>>(
          `${this.baseUrl}/supplier-clothes`,
          { params : filters }
        )
      );

      return result;
    }catch(err : any) {
      return Result.failure(extractErrorMessage(err.error));
    }
  }

}
