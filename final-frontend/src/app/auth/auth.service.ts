import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, of, tap } from 'rxjs';

import {
  AuthUser,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  MeResponse,
} from './auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:5125/api/Auth';

  private readonly tokenKey = 'salesbuzz_token';

  private readonly userKey = 'salesbuzz_user';

  private readonly _currentUser = new BehaviorSubject<AuthUser | null>(
    this._readUserFromStorage(),
  );

  public currentUser$ = this._currentUser.asObservable();

  constructor() {
    if (this.getToken()) {
      this.refreshCurrentUser().subscribe();
    }
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem(this.userKey, JSON.stringify(response.user));
        this._currentUser.next(response.user);
      }),
    );
  }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.baseUrl}/register`, request);
  }

  me(): Observable<MeResponse> {
    const token = this.getToken();
    const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
    return this.http.get<MeResponse>(`${this.baseUrl}/me`, { headers });
  }

  changePassword(payload: { currentPassword: string; newPassword: string; confirmPassword: string; }) {
    const token = this.getToken();
    const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
    return this.http.post(`${this.baseUrl}/change-password`, payload, { headers });
  }


  refreshCurrentUser(): Observable<MeResponse | null> {
    if (!this.getToken()) {
      this._currentUser.next(null);
      return of(null);
    }

    return this.me().pipe(
      tap((me) => {
        if (me && me.username) {
          const user: AuthUser = {
            id: parseInt(me.userId || '0', 10) || 0,
            username: me.username,
            role: me.role || 'User',
          };

          localStorage.setItem(this.userKey, JSON.stringify(user));
          this._currentUser.next(user);
        } else {
          // Clear if server reports unauthenticated
          localStorage.removeItem(this.tokenKey);
          localStorage.removeItem(this.userKey);
          this._currentUser.next(null);
        }
      }),
      catchError((error: HttpErrorResponse) => {
        // Keep the cached session for transient server/network failures.
        if (error.status === 401 || error.status === 403) {
          localStorage.removeItem(this.tokenKey);
          localStorage.removeItem(this.userKey);
          this._currentUser.next(null);
        }
        return of(null);
      }),
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this._currentUser.next(null);
  }

  getToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);

    if (token && this.isTokenExpired(token)) {
      this.clearStoredSession();
      return null;
    }

    return token;
  }

  private _readUserFromStorage(): AuthUser | null {
    const value = localStorage.getItem(this.userKey);
    if (!value) {
      return null;
    }

    try {
      return JSON.parse(value) as AuthUser;
    } catch {
      localStorage.removeItem(this.userKey);
      return null;
    }
  }

  getUser(): AuthUser | null {
    return this._currentUser.value;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return this.getUser()?.role?.toLowerCase() === 'admin';
  }

  private isTokenExpired(token: string): boolean {
    const parts = token.split('.');

    if (parts.length !== 3) {
      return true;
    }

    try {
      const payload = JSON.parse(this.decodeBase64Url(parts[1])) as {
        exp?: number;
      };

      return typeof payload.exp !== 'number' || payload.exp * 1000 <= Date.now();
    } catch {
      return true;
    }
  }

  private decodeBase64Url(value: string): string {
    const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
    return atob(padded);
  }

  private clearStoredSession(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this._currentUser.next(null);
  }
}
