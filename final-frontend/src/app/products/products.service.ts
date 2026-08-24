import {
  Injectable,
  inject
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  AuthService
} from '../auth/auth.service';

import {
  Product,
  CreateProductRequest
} from './products.model';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {

  private readonly http =
    inject(HttpClient);

  private readonly authService =
    inject(AuthService);

  private readonly baseUrl =
    'http://localhost:5125/api/Products';

  // -----------------------------------------
  // GET ALL PRODUCTS
  // -----------------------------------------

  getProducts(): Observable<Product[]> {

    const token =
      this.authService.getToken();

    return this.http.get<Product[]>(
      this.baseUrl,
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

  // -----------------------------------------
  // GET ONE PRODUCT
  // -----------------------------------------

  getProduct(
    id: number
  ): Observable<Product> {

    const token =
      this.authService.getToken();

    return this.http.get<Product>(
      `${this.baseUrl}/${id}`,
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

  // -----------------------------------------
  // CREATE PRODUCT
  // -----------------------------------------

  createProduct(
    request: CreateProductRequest
  ): Observable<Product> {

    const token =
      this.authService.getToken();

    return this.http.post<Product>(
      this.baseUrl,
      request,
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

  // -----------------------------------------
  // UPDATE PRODUCT
  // -----------------------------------------
  updateProduct(
    id: number,
    request: CreateProductRequest
  ): Observable<Product> {
    return this.http.put<Product>(
      `${this.baseUrl}/${id}`,
      request
    );
  }

  // -----------------------------------------
  // DELETE PRODUCT
  // -----------------------------------------
  deleteProduct(id: number): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}/${id}`
    );
  }
}
