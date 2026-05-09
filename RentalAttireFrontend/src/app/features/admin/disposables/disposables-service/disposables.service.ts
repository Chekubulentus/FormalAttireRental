import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { HttpClient } from '@angular/common/http';
import { Result } from '../../../../data/models/Results/result';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { ArchivedEntity } from '../../../../data/models/DTOs/Disposables/archive-entity';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DisposablesService {
  private disposablesUrl = `${BaseApiUrl}/Disposables`;
  constructor(private httpClient: HttpClient) {}

  async getAllArchivedRecordsAsync(
    currentPage: number,
    itemsPerPage: number,
  ): Promise<Result<PagedResult<ArchivedEntity>>> {
    try {
      var result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<ArchivedEntity>>>(
          `${this.disposablesUrl}`,
        ),
      );

      return result;
    } catch(err: any) {
      return Result.failure(err.error);
    }
  }
}
