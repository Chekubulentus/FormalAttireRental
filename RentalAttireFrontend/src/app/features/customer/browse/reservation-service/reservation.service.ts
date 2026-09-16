import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { RentalTransactionRequest } from '../../../../data/models/DTOs/Rentals/rental-transaction-request';
import { Result } from '../../../../data/models/Results/result';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private baseUrl = `${BaseApiUrl}/Rental`;

  constructor(private httpClient: HttpClient) { }
  

  async rentalReservationAsync(
    request: RentalTransactionRequest
  ) : Promise<Result<any>> {
    try {
      const result = await firstValueFrom(
        this.httpClient.post<Result<any>>(`${this.baseUrl}`, request)
      );

      return result;
    }catch(err: any) {
      return Result.failure(err.error.detail);
    }
  }
}
