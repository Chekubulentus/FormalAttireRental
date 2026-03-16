import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Result } from '../../../../data/models/Results/result';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { firstValueFrom } from 'rxjs';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { CreateClotheCommand } from '../create-clothe/create-clothe.component';

@Injectable({
  providedIn: 'root'
})
export class ClotheService {
  private clotheUrl = `${BaseApiUrl}/Clothe`

  constructor(
    private httpClient : HttpClient
  ) { }

  async filterClothesAsync(
    searchQuery : string,
    condition : string,
    gender : string,
    category : string,
    currentPage : number,
    itemsPerPage : number,
  ) : Promise<Result<PagedResult<ClotheDTO>>> {
    try {
      const filters = {
        searchQuery,
        condition,
        gender,
        category,
        currentPage,
        itemsPerPage
      };

      var result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<ClotheDTO>>>(
          `${this.clotheUrl}/filter-clothes`, { params : filters }
        )
      );
      return result;
    }catch(err : any) {
      return Result.failure(err.error);
    }
  }

  async createClotheAsync(
    command : FormData
  ) : Promise<Result<boolean>> {
    try { 
      var result = await firstValueFrom(
        this.httpClient.post<Result<boolean>>(`${this.clotheUrl}`, command)
      );

      return result;
    }catch(err : any) {
      return Result.failure(err.error);
    }
  }

}
