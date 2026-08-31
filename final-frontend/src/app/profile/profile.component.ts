import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../auth/auth.service';
import { OrderService } from '../orders/order.service';
import { QrService } from '../shared/qr.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css'],
})
export class ProfileComponent implements OnInit {
  user: any = null;
  me: any = null;
  orders: any[] = [];
  selectedOrder: any = null;
  loadingOrders = false;
  error = '';

  // change password model
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  changingPassword = false;
  changePasswordMessage = '';

  qrImage: string | null = null;
  showQrModal = false;

  constructor(
    private readonly auth: AuthService,
    private readonly orderService: OrderService,
    private readonly qrService: QrService,
    private readonly router: Router,
  ) {}

  ngOnInit(): void {
    this.user = this.auth.getUser();

    // fetch fresh me info
    this.auth.me().subscribe({
      next: (m) => {
        this.me = m;
      },
      error: () => {
        // ignore
      },
    });

    this.loadMyOrders();
  }

  loadMyOrders(): void {
    this.loadingOrders = true;
    this.orderService.getMyOrders().subscribe({
      next: (list) => {
        this.orders = (list || []).map((order: any) => ({
          ...order,
          orderDate:
            order.orderDate ??
            order.OrderDate ??
            order.createdAt ??
            order.CreatedAt ??
            null,
        }));
        this.loadingOrders = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to load your orders.';
        this.loadingOrders = false;
      },
    });
  }

  viewOrder(orderId: number): void {
    void this.router.navigate(['/order-details'], { queryParams: { id: orderId } });
  }

  async openQrForOrder(order: any): Promise<void> {
    this.selectedOrder = order;
    const orderUrl = `${window.location.origin}/order-details?id=${order.id}`;
    try {
      this.qrImage = await this.qrService.toDataUrl(orderUrl, { width: 300 });
    } catch (e) {
      console.error('QR generation failed', e);
      this.qrImage = `https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=${encodeURIComponent(orderUrl)}`;
    }
    this.showQrModal = true;
  }

  closeQr(): void {
    this.showQrModal = false;
    this.qrImage = null;
    this.selectedOrder = null;
  }

  printQr(): void {
    if (!this.qrImage) return;
    const win = window.open('', '_blank', 'width=400,height=500');
    if (!win) return;
    win.document.write(`
      <html><head><title>Print QR</title>
      <style>body{text-align:center;margin:0;padding:20px;}img{max-width:100%;height:auto;}</style>
      </head><body>
      <img src="${this.qrImage}" alt="Order QR" />
      <script>window.onload = function(){ setTimeout(function(){ window.print(); }, 250); };</script>
      </body></html>
    `);
    win.document.close();
  }

  changePassword(): void {
    if (!this.currentPassword || !this.newPassword || !this.confirmPassword) {
      this.changePasswordMessage = 'All fields are required.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.changePasswordMessage = 'New password and confirmation do not match.';
      return;
    }

    this.changingPassword = true;
    this.changePasswordMessage = '';

    this.auth.changePassword({
      currentPassword: this.currentPassword,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword,
    }).subscribe({
      next: (res: any) => {
        this.changePasswordMessage = res?.message ?? 'Password changed.';
        this.changingPassword = false;
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
      },
      error: (err) => {
        console.error(err);
        this.changePasswordMessage = err?.error?.message ?? 'Failed to change password.';
        this.changingPassword = false;
      },
    });
  }
}
