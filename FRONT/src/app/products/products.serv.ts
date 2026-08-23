import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { PublicApiClient } from '@salesbuzz/public-sdk';

import {
  CreateProductRequest,
  Product,
  UpdateProductRequest,
} from './products.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly apiClient = inject(PublicApiClient);

  private readonly productsUrl =
    'http://localhost:5125/api/Products';

  getProducts(): Observable<Product[]> {
    return this.apiClient.get<Product[]>(
      this.productsUrl,
      {
        withAuth: true,
      },
    );
  }

  getProduct(id: number): Observable<Product> {
    return this.apiClient.get<Product>(
      `${this.productsUrl}/${id}`,
      {
        withAuth: true,
      },
    );
  }

  createProduct(
    product: CreateProductRequest,
  ): Observable<Product> {
    return this.apiClient.post<Product>(
      this.productsUrl,
      product,
      {
        withAuth: true,
      },
    );
  }

  updateProduct(
    id: number,
    product: UpdateProductRequest,
  ): Observable<Product> {
    return this.apiClient.put<Product>(
      `${this.productsUrl}/${id}`,
      product,
      {
        withAuth: true,
      },
    );
  }

  deleteProduct(id: number): Observable<void> {
    return this.apiClient.delete<void>(
      `${this.productsUrl}/${id}`,
      {
        withAuth: true,
      },
    );
  }
}
