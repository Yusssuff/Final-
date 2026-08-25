import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Order } from './order.model';
import { OrderService } from './order.service';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-detail.html',
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;
  isLoading = true;
  error = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly orderService: OrderService,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.queryParamMap.get('id'));

    if (!id || Number.isNaN(id)) {
      this.error = 'Order id is missing or invalid.';
      this.isLoading = false;
      return;
    }

    this.orderService.getOrder(id).subscribe({
      next: (result) => {
        this.order = result;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load order details.';
        this.isLoading = false;
      },
    });
  }
}
