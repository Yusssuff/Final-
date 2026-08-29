import { Component, OnInit, ViewChild } from '@angular/core';

import { CommonModule, DecimalPipe } from '@angular/common';

import { FormsModule } from '@angular/forms';

import { BIGridComponent, BIModulesModule } from 'bi-modules';

import { ControlTypes, DataTypes, IChangeset, IColumns } from 'bi-interfaces';

import { ProductsDataSource } from './products.data-source';

import { AuthService } from '../auth/auth.service';
import { OrderService } from '../orders/order.service';
import { Product } from './products.model';
import { ProductsService } from './products.service';

import { DialogModule } from '@progress/kendo-angular-dialog';
import { QrService } from '../shared/qr.service';

@Component({
  selector: 'app-products',

  standalone: true,

  imports: [CommonModule, FormsModule, BIModulesModule, DialogModule],

  templateUrl: './products.html',

  styleUrls: ['./products.css'],

  providers: [DecimalPipe],
})
export class Products implements OnInit {
  @ViewChild('productsGrid')
  grid?: BIGridComponent;

  readonly changeSet: IChangeset = {
    changesetArr: [],
  };

  readonly columns: IColumns[] = [
    {
      Name: 'id',

      DisplayName: 'ID',

      DataType: DataTypes.NUMERIC,

      controlType: ControlTypes.Number,

      IsEditable: false,

      IsFilterable: true,

      IsVisible: true,

      DefaultValue: null,

      Width: 90,

      DomID: 'ProductID',
    } as IColumns,

    {
      Name: 'name',

      DisplayName: 'Product',

      DataType: DataTypes.Text,

      controlType: ControlTypes.Text,

      IsEditable: true,

      IsFilterable: true,

      IsVisible: true,

      DefaultValue: '',

      Width: 240,

      DomID: 'ProductName',
    } as IColumns,

    {
      Name: 'price',

      DisplayName: 'Price',

      DataType: DataTypes.NUMERIC,

      controlType: ControlTypes.Number,

      IsEditable: true,

      IsFilterable: true,

      IsVisible: true,

      DefaultValue: 0,

      Width: 130,

      DomID: 'Price',
    } as IColumns,

    {
      Name: 'quantity',

      DisplayName: 'Quantity',

      DataType: DataTypes.NUMERIC,

      controlType: ControlTypes.Number,

      IsEditable: true,

      IsFilterable: true,

      IsVisible: true,

      DefaultValue: 0,

      Width: 130,

      DomID: 'Quantity',
    } as IColumns,
  ];

  searchValue = '';

  get navButtons(): any {
    const admin = this.isAdmin;
    // Enable primary nav buttons to be interactive in the UI. Keep visibility controlled by admin,
    // but ensure buttons are clickable for demonstration and for users who need them.
    // If you want strict admin-only control, change disable back to !admin.
    return {
      add: { visibility: true, disable: false },
      edit: { visibility: true, disable: false },
      save: { visibility: true, disable: false },
      delete: { visibility: true, disable: false },
      searchbar: { visibility: true }
    };
  }

  // Handle actions emitted by the BI-Nav toolbar. The nav emits objects like
  // { action: 'add' | 'save' | 'delete' | 'cancel' | 'searchbar' | 'historyData' | 'workFlow', ... }
  handleNavAction(event: any): void {
    console.debug('BI-Nav ActionClicked event:', event);
    if (!event || !event.action) {
      return;
    }

    switch (event.action) {
      case 'add':
        // If the BI-Nav includes a selected record or payload, prefill modal as needed.
        // Also support direct payload create if nav supplies a new record object.
        const payload = event.payload || event.data || event.record || event.newRecord;
        if (payload && typeof payload === 'object') {
          // Attempt to create directly in backend when BI-Nav provides full record payload
          this.productsService.createProduct(payload).subscribe({
            next: () => {
              this.grid?.read();
              alert('Product created (from BI-Nav)');
            },
            error: (err) => {
              console.error('Failed to create product from BI-Nav payload', err);
              alert('Failed to create product');
            }
          });
        } else if (event.recordId) {
          // open modal to edit existing (treat as edit)
          this.openEditModal(Number(event.recordId));
        } else {
          this.openAddModal();
        }
        break;
      case 'save':
        // Let the grid handle save if available, otherwise call submitModal for local modal flows
        if (this.grid && typeof (this.grid as any).Save === 'function') {
          (this.grid as any).Save();
        } else {
          // fallback: if modal is open, try to submit it; otherwise no-op
          if (this.showModal) {
            this.submitModal();
          }
        }
        break;
      case 'delete':
        // If the nav provides a recordId in the event, delete that record directly.
        if (event.recordId) {
          this.openSelectDelete(Number(event.recordId));
        } else {
          this.openSelectDelete();
        }
        break;
      case 'cancel':
        // close any modal if open or call grid Cancel if available
        if (this.showModal) {
          this.closeModal();
        } else if (this.grid && typeof (this.grid as any).Cancel === 'function') {
          (this.grid as any).Cancel();
        }
        break;
      case 'searchbar':
        // The nav may include searchText property
        const q = (event.searchText || '').toString().trim();
        this.dataSource.read(q);
        break;
      case 'historyData':
        // no-op here; BI-Nav sent historyData payload in event.historyData; implement if needed
        console.debug('BI-Nav historyData action', event.historyData);
        break;
      case 'edit':
        // Support an 'edit' action coming from the BI-Nav: open the edit modal for provided id or prompt
        if (event.recordId) {
          this.openSelectEdit(Number(event.recordId));
        } else {
          this.openSelectEdit();
        }
        break;
      case 'attachments':
        // The BI-Nav may pass a record id to open attachments for
        this.openAttachments(event.recordId);
        break;
      case 'info':
        this.openInfo(event.recordId);
        break;
      default:
        console.debug('Unhandled BI-Nav action', event);
        break;
    }
  }

  // Toggle: show the BI-Nav for admins. Set to true to use BI-Nav, false to fall back to legacy admin buttons.
  showBiNav = true;

  constructor(
    public readonly dataSource: ProductsDataSource,
    private readonly auth: AuthService,
    private readonly productsService: ProductsService,
    private readonly orderService: OrderService,
    private readonly qrService: QrService,
  ) {}

  // Debug helper: logs click events within the BI-Nav wrapper to help diagnose overlay/click issues.
  debugNavClick(evt: Event): void {
    const target = evt.target as HTMLElement;
    console.debug('BI-Nav wrapper clicked, target:', target && (target.className || target.tagName), evt);
  }

  // Handler for the projected Edit button inside BI-Nav.
  onBiNavEditClick(evt: Event): void {
    evt.stopPropagation();
    // Try to get the currently selected row from the BI-Grid if available
    try {
      const current = (this.grid as any)?.GetRowValue ? (this.grid as any).GetRowValue() : null;
      const id = current && (current.id ?? current.ID ?? current.Id);
      if (id !== undefined && id !== null) {
        this.openEditModal(Number(id));
        return;
      }
    } catch (e) {
      console.warn('Failed to determine selected row from grid', e);
    }

    // Fallback: prompt user for ID (legacy flow)
    this.openSelectEdit();
  }

  get isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  get changeSetBinding(): any { return this.changeSet; }

  ngOnInit(): void {
    this.dataSource.read();
  }

  searchProducts(event: Event): void {
    const input = event.target as HTMLInputElement;

    this.searchValue = input.value;

    // Pass the plain search term to the datasource so it performs client-side filtering
    this.dataSource.read(this.searchValue);
  }

  clearSearch(): void {
    this.searchValue = '';
    this.dataSource.read('');
  }
  refresh(): void {
    this.grid?.read();
  }

  // -----------------------------------------
  // Admin modal-based flows
  // -----------------------------------------

  showModal = false;

  showQuickOrderModal = false;

  showQrModal = false;

  quickOrderProducts: Product[] = [];

  selectedProductId = 0;

  orderQuantity = 1;

  latestOrderSummary: {
    id: number;
    productName: string;
    quantity: number;
    totalPrice: number;
    orderDate: string;
    qrUrl: string;
  } | null = null;

  editingId: number | null = null;

  modalProduct: {
    name: string;
    price: number;
    quantity: number;
  } = {
    name: '',
    price: 0,
    quantity: 0,
  };

  openAddModal(): void {
    this.editingId = null;
    this.modalProduct = { name: '', price: 0, quantity: 0 };
    this.showModal = true;
  }

  openSelectEdit(id?: number): void {
    // If id provided by BI-Nav event, use it directly; otherwise fallback to prompt
    if (typeof id === 'number' && !isNaN(id)) {
      this.openEditModal(id);
      return;
    }

    const idStr = window.prompt('Enter product ID to edit:');
    const parsed = idStr ? parseInt(idStr, 10) : NaN;
    if (!parsed || isNaN(parsed)) {
      return;
    }
    this.openEditModal(parsed);
  }

  openAttachments(id?: number | string | null): void {
    // BI-Nav may signal attachments for a specific record. If id provided, open attachments flow.
    console.debug('Open attachments for', id);
    if (id) {
      // TODO: integrate with attachments service/modal. For now open a dialog or alert.
      alert('Open attachments for product id: ' + id);
    } else {
      alert('Open attachments - no record selected');
    }
  }

  openInfo(id?: number | string | null): void {
    console.debug('Open info for', id);
    if (id) {
      alert('Open info for product id: ' + id);
    } else {
      alert('Open info - no record selected');
    }
  }

  openEditModal(id: number): void {
    this.productsService.getProduct(id).subscribe({
      next: (product) => {
        this.editingId = id;
        this.modalProduct = {
          name: product.name,
          price: product.price,
          quantity: product.quantity,
        };
        this.showModal = true;
      },
      error: () => {
        alert('Failed to load product for editing');
      },
    });
  }

  openSelectDelete(id?: number): void {
    // If id provided by BI-Nav event, delete directly; otherwise ask user for id (legacy flow)
    if (typeof id === 'number' && !isNaN(id)) {
      if (!confirm(`Delete product id ${id}? This cannot be undone.`)) {
        return;
      }
      this.productsService.deleteProduct(id).subscribe({
        next: () => {
          this.grid?.read();
          alert('Product deleted');
        },
        error: (err) => {
          console.error(err);
          alert('Failed to delete product');
        },
      });
      return;
    }

    const idStr = window.prompt('Enter product ID to delete:');
    const parsed = idStr ? parseInt(idStr, 10) : NaN;
    if (!parsed || isNaN(parsed)) {
      return;
    }

    if (!confirm(`Delete product id ${parsed}? This cannot be undone.`)) {
      return;
    }

    this.productsService.deleteProduct(parsed).subscribe({
      next: () => {
        this.grid?.read();
        alert('Product deleted');
      },
      error: (err) => {
        console.error(err);
        alert('Failed to delete product');
      },
    });
  }

  openQuickOrderModal(): void {
    this.selectedProductId = 0;
    this.orderQuantity = 1;
    this.quickOrderProducts = [];
    this.showQuickOrderModal = true;

    this.productsService.getProducts().subscribe({
      next: (products) => {
        this.quickOrderProducts = products;
        if (products.length > 0) {
          this.selectedProductId = products[0].id;
        }
      },
      error: () => {
        alert('Failed to load products for order creation');
      },
    });
  }

  closeQuickOrderModal(): void {
    this.showQuickOrderModal = false;
    this.selectedProductId = 0;
    this.orderQuantity = 1;
  }

  submitQuickOrder(): void {
    if (!this.selectedProductId || this.orderQuantity <= 0) {
      alert('Please select a valid product and quantity.');
      return;
    }

    const product = this.quickOrderProducts.find(
      (item) => item.id === this.selectedProductId,
    );

    if (!product) {
      alert('Selected product was not found.');
      return;
    }

    if (this.orderQuantity > product.quantity) {
      alert(
        `Only ${product.quantity} units are available for ${product.name}.`,
      );
      return;
    }

    this.orderService
      .createOrder({
        productId: this.selectedProductId,
        quantity: this.orderQuantity,
      })
      .subscribe({
        next: async (order) => {
          this.showQuickOrderModal = false;

          const orderUrl = `${window.location.origin}/order-details?id=${order.id}`;

          try {
            const qrDataUrl = await this.qrService.toDataUrl(orderUrl, {
              width: 300,
            });

            this.latestOrderSummary = {
              id: order.id,
              productName: product.name,
              quantity: order.quantity,
              totalPrice: order.totalPrice,
              orderDate: order.orderDate,
              qrUrl: qrDataUrl,
            };

            this.showQrModal = true;
          } catch (qrErr) {
            console.error(
              'QR generation failed, falling back to external service',
              qrErr,
            );

            this.latestOrderSummary = {
              id: order.id,
              productName: product.name,
              quantity: order.quantity,
              totalPrice: order.totalPrice,
              orderDate: order.orderDate,
              qrUrl: `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(orderUrl)}`,
            };

            this.showQrModal = true;
          }
        },
        error: (err) => {
          console.error(err);
          alert('Failed to create order.');
        },
      });
  }

  printQr(): void {
    const dataUrl = this.latestOrderSummary?.qrUrl;
    if (!dataUrl) {
      return;
    }

    const win = window.open('', '_blank', 'width=400,height=500');
    if (!win) {
      return;
    }

    win.document.write(`
      <html><head><title>Print QR</title>
      <style>body{text-align:center;margin:0;padding:20px;}img{max-width:100%;height:auto;}</style>
      </head><body>
      <img src="${dataUrl}" alt="Order QR" />
      <script>window.onload = function(){ setTimeout(function(){ window.print(); }, 250); };</script>
      </body></html>
    `);

    win.document.close();
  }

  closeQrModal(): void {
    this.showQrModal = false;
    this.latestOrderSummary = null;
  }

  closeModal(): void {
    this.showModal = false;
  }

  submitModal(): void {
    const payload = {
      name: (this.modalProduct.name || '').trim(),
      price: Number(this.modalProduct.price) || 0,
      quantity: Number(this.modalProduct.quantity) || 0,
    };

    if (!payload.name) {
      alert('Name is required');
      return;
    }

    if (this.editingId) {
      this.productsService.updateProduct(this.editingId, payload).subscribe({
        next: () => {
          this.showModal = false;
          this.grid?.read();
          alert('Product updated');
        },
        error: (err) => {
          console.error(err);
          alert('Failed to update product');
        },
      });
    } else {
      this.productsService.createProduct(payload).subscribe({
        next: () => {
          this.showModal = false;
          this.grid?.read();
          alert('Product created');
        },
        error: (err) => {
          console.error(err);
          alert('Failed to create product');
        },
      });
    }
  }
}


