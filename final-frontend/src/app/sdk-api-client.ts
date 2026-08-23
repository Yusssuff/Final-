import { Injectable, inject } from '@angular/core';
import {
  HttpClient,
  HttpHeaders,
  HttpParams
} from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  PublicApiClient,
  PublicApiRequestOptions
} from '@salesbuzz/public-sdk';

@Injectable({
  providedIn: 'root'
})
export class SalesBuzzApiClient extends PublicApiClient {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:5113';
  private readonly tokenKey = 'salesbuzz_token';

  override get<T>(
    url: string,
    options?: PublicApiRequestOptions
  ): Observable<T> {
    return this.http.get<T>(
      this.buildUrl(url),
      this.buildOptions(options)
    );
  }

  override post<T>(
    url: string,
    body: unknown,
    options?: PublicApiRequestOptions
  ): Observable<T> {
    return this.http.post<T>(
      this.buildUrl(url),
      body,
      this.buildOptions(options)
    );
  }

  override put<T>(
    url: string,
    body: unknown,
    options?: PublicApiRequestOptions
  ): Observable<T> {
    return this.http.put<T>(
      this.buildUrl(url),
      body,
      this.buildOptions(options)
    );
  }

  override patch<T>(
    url: string,
    body: unknown,
    options?: PublicApiRequestOptions
  ): Observable<T> {
    return this.http.patch<T>(
      this.buildUrl(url),
      body,
      this.buildOptions(options)
    );
  }

  override delete<T>(
    url: string,
    options?: PublicApiRequestOptions
  ): Observable<T> {
    return this.http.delete<T>(
      this.buildUrl(url),
      this.buildOptions(options)
    );
  }

  private buildUrl(url: string): string {
    if (
      url.startsWith('http://') ||
      url.startsWith('https://')
    ) {
      return url;
    }

    return `${this.baseUrl}/${url.replace(/^\/+/, '')}`;
  }

  private buildOptions(
    options?: PublicApiRequestOptions
  ): {
    headers: HttpHeaders;
    params: HttpParams;
  } {
    let headers = new HttpHeaders(
      options?.headers ?? {}
    );

    if (options?.withAuth) {
      const token =
        typeof localStorage !== 'undefined'
          ? localStorage.getItem(this.tokenKey)
          : null;

      if (token) {
        headers = headers.set(
          'Authorization',
          `Bearer ${token}`
        );
      }
    }

    let params = new HttpParams();

    for (
      const [key, value]
      of Object.entries(options?.params ?? {})
    ) {
      if (
        value !== null &&
        value !== undefined
      ) {
        params = params.set(
          key,
          String(value)
        );
      }
    }

    return {
      headers,
      params
    };
  }
}
