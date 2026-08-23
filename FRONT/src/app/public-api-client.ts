import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';

import { PublicApiClient, PublicApiRequestOptions } from '@salesbuzz/public-sdk';

@Injectable({
  providedIn: 'root',
})
export class AppPublicApiClient extends PublicApiClient {
  private readonly http = inject(HttpClient);

  override get<T>(url: string, options?: PublicApiRequestOptions): Observable<T> {
    return this.http.get<T>(url, {
      headers: options?.headers,
      params: this.toHttpParams(options?.params),
    });
  }

  override post<T>(url: string, body: unknown, options?: PublicApiRequestOptions): Observable<T> {
    return this.http.post<T>(url, body, {
      headers: options?.headers,
      params: this.toHttpParams(options?.params),
    });
  }

  override put<T>(url: string, body: unknown, options?: PublicApiRequestOptions): Observable<T> {
    return this.http.put<T>(url, body, {
      headers: options?.headers,
      params: this.toHttpParams(options?.params),
    });
  }

  override patch<T>(url: string, body: unknown, options?: PublicApiRequestOptions): Observable<T> {
    return this.http.patch<T>(url, body, {
      headers: options?.headers,
      params: this.toHttpParams(options?.params),
    });
  }

  override delete<T>(url: string, options?: PublicApiRequestOptions): Observable<T> {
    return this.http.delete<T>(url, {
      headers: options?.headers,
      params: this.toHttpParams(options?.params),
    });
  }

  private toHttpParams(
    params?: Record<string, string | number | boolean | null | undefined>,
  ): Record<string, string> | undefined {
    if (!params) {
      return undefined;
    }

    const result: Record<string, string> = {};

    for (const [key, value] of Object.entries(params)) {
      if (value !== null && value !== undefined) {
        result[key] = String(value);
      }
    }

    return result;
  }
}
