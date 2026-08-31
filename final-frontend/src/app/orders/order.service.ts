import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, catchError, map } from 'rxjs';

import { AuthService } from '../auth/auth.service';
import { CreateOrderRequest, Order } from './order.model';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly baseUrl = 'http://localhost:5125/api/Order';

  private readonly headersForRequest = (): HttpHeaders => {
    const token = this.authService.getToken();
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  };

  createOrder(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.baseUrl, request, {
      headers: this.headersForRequest(),
    }).pipe(
      map((order) => this.normalizeOrder(order)),
      catchError((err) => {
        console.warn('createOrder failed', err);
        throw err;
      })
    );
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/${id}`, {
      headers: this.headersForRequest(),
    }).pipe(
      map((order) => this.normalizeOrder(order)),
      catchError((err) => {
        console.warn('getOrder failed', err);
        throw err;
      })
    );
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.baseUrl}/my`, {
      headers: this.headersForRequest(),
    }).pipe(
      map((orders) => orders.map((order) => this.normalizeOrder(order))),
      catchError((err) => {
        console.warn('getMyOrders failed, returning empty array', err);
        return of([] as Order[]);
      })
    );
  }

  private normalizeOrder(order: Order): Order {
    const orderDate = order.orderDate;

    if (
      typeof orderDate === 'string' &&
      orderDate.length > 0 &&
      !/(?:Z|[+-]\d{2}:?\d{2})$/i.test(orderDate)
    ) {
      return {
        ...order,
        orderDate: `${orderDate}Z`,
      };
    }

    return order;
  }
}
