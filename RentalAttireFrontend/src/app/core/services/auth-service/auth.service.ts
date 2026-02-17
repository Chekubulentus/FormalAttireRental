import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccessTokenKey } from '../../../../environments/access-token-key';
import { RefreshTokenKey } from '../../../../environments/refresh-token-key';
import { AuthenticationResult } from '../../../shared/models/authentication-result';
import { Result } from '../../../shared/models/result';
import { BaseApiUrl } from '../../../../environments/base-api-url';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private httpClient: HttpClient,
    private router: Router,
  ) {}

  private baseUrl = `${BaseApiUrl}/Authentication`;
  private accessTokenKey = `${AccessTokenKey}`;
  private refreshTokenKey = `${RefreshTokenKey}`;

  logout() {
    this.removeTokens();
    this.router.navigateByUrl('/log-in');
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  saveTokens(access: string, refresh: string) {
    localStorage.setItem(this.accessTokenKey, access);
    localStorage.setItem(this.refreshTokenKey, refresh);
  }

  removeTokens() {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
  }

  getCurrentUser() {}

  async login(
    email: string,
    password: string,
  ): Promise<Result<AuthenticationResult>> {
    try {
      const response = await firstValueFrom(
        this.httpClient.post<Result<AuthenticationResult>>(`${this.baseUrl}`, {
          email: email,
          password: password,
        }),
      );
      return response;
    } catch (err: any) {
      console.log('Full error object:', err);
      console.log('Error body:', err?.error);
      const message = err?.error;
      return Result.failure(message);
    }
  }
}
