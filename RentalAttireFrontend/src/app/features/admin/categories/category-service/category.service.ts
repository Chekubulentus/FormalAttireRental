import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiUrl } from '../../../../../environments/base-api-url';
import { Result } from '../../../../data/models/Results/result';
import { Category } from '../../../../data/models/DTOs/Category/category';
import { filter, firstValueFrom } from 'rxjs';
import { PagedResult } from '../../../../data/models/Results/pagedResult';
import { ArchiveCategoryCommand } from '../../../../data/models/DTOs/Category/archive-category-command';
import { CreateCategoryCommand } from '../../../../data/models/DTOs/Category/create-category';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private categoryUrl = `${BaseApiUrl}/Category`;

  constructor(private httpClient: HttpClient) {}

  async getCategoryByIdAsync(categoryId: number): Promise<Result<Category>> {
    try {
      var result = await firstValueFrom(
        this.httpClient.get<Result<Category>>(
          `${this.categoryUrl}/${categoryId}`,
        ),
      );

      return result;
    } catch (err: any) {
      return Result.failure(err.error);
    }
  }

  async filterCategoriesAsync(
    searchQuery: string,
    currentPage: number,
    itemsPerPage: number,
  ): Promise<Result<PagedResult<Category>>> {
    const filters = {
      searchQuery,
      currentPage,
      itemsPerPage,
    };

    try {
      var result = await firstValueFrom(
        this.httpClient.get<Result<PagedResult<Category>>>(
          `${this.categoryUrl}/filter-categories`,
          { params: filters },
        ),
      );

      return result;
    } catch (eryy: any) {
      return Result.failure(eryy.error);
    }
  }

  async archiveCategoryByIdAsync(
    commnad : ArchiveCategoryCommand
  ) : Promise<Result<boolean>> {
    try {
      var result = await firstValueFrom(
        this.httpClient.patch<Result<boolean>>(`${this.categoryUrl}`, { commnad})
      );

      return result;
    }catch(err : any) {
      return Result.failure(err.error);
    }
  }

  async createCategoryAsync(
    command : CreateCategoryCommand
  ) : Promise<Result<boolean>> {
    try {
      var result = await firstValueFrom(
        this.httpClient.post<Result<boolean>>(`${this.categoryUrl}`, command)
      );

      return result;
    }catch(err : any) {
      return Result.failure(err.error);
    }
  }
}