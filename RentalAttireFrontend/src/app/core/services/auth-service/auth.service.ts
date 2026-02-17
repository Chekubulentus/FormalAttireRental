import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccessTokenKey } from '../../../../environments/access-token-key';
import { RefreshTokenKey } from '../../../../environments/refresh-token-key';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(
    private httpClient : HttpClient,
    private router : Router
  ) { }

  private accessTokenKey = `${AccessTokenKey}`;
  private refreshTokenKey = `${RefreshTokenKey}`;

  logout() {
    this.removeTokens();
    this.router.navigateByUrl('/log-in');
  }

  getAccessToken() : string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken() : string | null {
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

}
