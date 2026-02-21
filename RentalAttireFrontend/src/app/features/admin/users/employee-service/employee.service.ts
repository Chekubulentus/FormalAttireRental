import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { HttpClient } from '@angular/common/http';
import { Result } from '../../../../data/models/Results/result';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { firstValueFrom } from 'rxjs';
import { EmployeeDTO } from '../../../../data/models/DTOs/Employees/employee-dto';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private employeeUrl = `${BaseApiUrl}/Employee`;

  constructor(private httpClient: HttpClient) {}

  async getAllEmployeesAsync(
    currentPage: number,
    itemsPerPage: number,
  ): Promise<Result<PagedResult<EmployeeDTO>>> {
    try {
      const result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<EmployeeDTO>>>(
          `${this.employeeUrl}?currentPage=${currentPage}&itemsPerPage=${itemsPerPage}`,
        ),
      );
      return result;
    } catch (err: any) {
      const message = err.error;
      console.log(message);
      return Result.failure(message);
    }
  }

  async searchEmployeeAsync(
    searchQuery: string,
    currentPage: number,
    itemsPerPage: number,
  ): Promise<Result<PagedResult<EmployeeDTO>>> {
    try {
      const result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<EmployeeDTO>>>(
          `${this.employeeUrl}/search-employee?searchQuery=${searchQuery}&currentPage=${currentPage}&itemsPerPage=${itemsPerPage}`,
        ),
      );
      return result;
    } catch (error: any) {
      const message = error.error;
      console.log(message);
      return Result.failure(message);
    }
  }
}
