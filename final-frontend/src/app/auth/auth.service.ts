import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import {
  AuthUser,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  MeResponse
} from './auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl =
    'http://localhost:5125/api/Auth';

  private readonly tokenKey =
    'salesbuzz_token';

  private readonly userKey =
    'salesbuzz_user';

  login(
    request: LoginRequest
  ): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(
        `${this.baseUrl}/login`,
        request
      )
      .pipe(
        tap(response => {
          localStorage.setItem(
            this.tokenKey,
            response.token
          );

          localStorage.setItem(
            this.userKey,
            JSON.stringify(response.user)
          );
        })
      );
  }

  register(
    request: RegisterRequest
  ): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(
      `${this.baseUrl}/register`,
      request
    );
  }

  me(): Observable<MeResponse> {
    const token = this.getToken();

    return this.http.get<MeResponse>(
      `${this.baseUrl}/me`,
      {
        headers: token
          ? {
              Authorization:
                `Bearer ${token}`
            }
          : {}
      }
    );
  }

  logout(): void {
    localStorage.removeItem(
      this.tokenKey
    );

    localStorage.removeItem(
      this.userKey
    );
  }

  getToken(): string | null {
    return localStorage.getItem(
      this.tokenKey
    );
  }

  getUser(): AuthUser | null {
    const value =
      localStorage.getItem(
        this.userKey
      );

    if (!value) {
      return null;
    }

    try {
      return JSON.parse(value) as AuthUser;
    } catch {
      localStorage.removeItem(
        this.userKey
      );

      return null;
    }
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return (
      this.getUser()?.role?.toLowerCase() ===
      'admin'
    );
  }
}
