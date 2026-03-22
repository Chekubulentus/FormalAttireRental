import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Result } from '../../../data/models/Results/result';
import { UserViewModel } from '../../../data/models/DTOs/Users/user-view-model';
import { firstValueFrom } from 'rxjs';
import { BaseApiUrl } from '../../../../environments/base-api-url';
import { BaseRouteReuseStrategy } from '@angular/router';
import { AuthService } from '../auth-service/auth.service';
import { CreateCategoryCommand } from '../../../data/models/DTOs/Category/create-category';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = `${BaseApiUrl}/User`;

  constructor(
    private httpClient: HttpClient,
    private authService : AuthService
  ) {}

  async getUserViewModelByIdAsync(id: number): Promise<Result<UserViewModel>> {
    try {
      const result = await firstValueFrom(
        this.httpClient.get<Result<UserViewModel>>(
          `${this.baseUrl}/user-view-model/${id}`,
        ),
      );
      return result;
    } catch (err: any) {
      console.log(`${err.error}`);
      const message = err.error;
      return Result.failure(message);
    }
  }

  async getCurrentUserViewModel(): Promise<Result<UserViewModel>> {
    try {
      const user = this.authService.getCurrentUser();

      if (!user) return Result.failure('User does not exist.');

      const parsedUser = JSON.parse(user);

      const result = await firstValueFrom(
        this.httpClient.get<Result<UserViewModel>>(
          `${this.baseUrl}/user-view-model/${parsedUser.id}`,
        ),
      );

      return result;
    } catch (err: any) {
      console.log(err.error);
      return Result.failure(err.error);
    }
  }
}
