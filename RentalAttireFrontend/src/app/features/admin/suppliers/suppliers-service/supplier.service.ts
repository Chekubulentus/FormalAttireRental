import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {
  private baseUrl = `${BaseApiUrl}/Supplier`;

  constructor(
    private httpClient : HttpClient
  ) { }

  
}
