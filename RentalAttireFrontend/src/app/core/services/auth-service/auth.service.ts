import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccessTokenKey } from '../../../../environments/access-token-key';
import { RefreshTokenKey } from '../../../../environments/refresh-token-key';
import { AuthenticationResult } from '../../../shared/models/authentication-result';
import { Result } from '../../../shared/models/result';
import { BaseApiUrl } from '../../../../environments/base-api-url';
import { firstValueFrom, Observable } from 'rxjs';
import { CurrentUser } from '../../../../environments/current-user';

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

  saveTokens(
    access: string | null | undefined,
    refresh: string | null | undefined,
  ) {
    if (access && refresh) {
      localStorage.setItem(this.accessTokenKey, access);
      localStorage.setItem(this.refreshTokenKey, refresh);
    }
  }

  removeTokens() {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
  }

  getCurrentUser() {
    return localStorage.getItem(CurrentUser);
  }

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
      localStorage.setItem(CurrentUser, JSON.stringify(response.data?.user));
      return response;
    } catch (err: any) {
      console.log('Error body:', err?.error);
      const message = err?.error;
      return Result.failure(message);
    }
  }

  refreshToken(): Observable<Result<AuthenticationResult>> {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();

    return this.httpClient.post<Result<AuthenticationResult>>(
      `${this.baseUrl}/refresh`,
      {
        accessToken: accessToken,
        refreshToken: refreshToken,
      },
    );
  }
}
