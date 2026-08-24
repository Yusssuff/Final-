import {
  Component,
  OnInit,
  ViewChild
} from '@angular/core';

import {
  CommonModule,
  DecimalPipe
} from '@angular/common';

import {
  FormsModule
} from '@angular/forms';

import {
  BIGridComponent,
  BIModulesModule
} from 'bi-modules';

import {
  ControlTypes,
  DataTypes,
  IChangeset,
  IColumns
} from 'bi-interfaces';

import {
  ProductsDataSource
} from './products.data-source';

import { AuthService } from '../auth/auth.service';
import { ProductsService } from './products.service';

@Component({
  selector: 'app-products',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    BIModulesModule
  ],

  templateUrl: './products.html',

styleUrls: ['./products.css'],

providers: [DecimalPipe]
})
export class Products
  implements OnInit {

  @ViewChild(
    'productsGrid'
  )
  grid?: BIGridComponent;

  readonly changeSet:
    IChangeset = {
      changesetArr: []
    };

  readonly columns:
    IColumns[] = [

      {
        Name: 'id',

        DisplayName: 'ID',

        DataType:
          DataTypes.NUMERIC,

        controlType:
          ControlTypes.Number,

        IsEditable: false,

        IsFilterable: true,

        IsVisible: true,

        DefaultValue: null,

        Width: 90,

        DomID: 'ProductID'

      } as IColumns,

      {
        Name: 'name',

        DisplayName: 'Product',

        DataType:
          DataTypes.Text,

        controlType:
          ControlTypes.Text,

        IsEditable: true,

        IsFilterable: true,

        IsVisible: true,

        DefaultValue: '',

        Width: 240,

        DomID: 'ProductName'

      } as IColumns,

      {
        Name: 'description',

        DisplayName: 'Description',

        DataType:
          DataTypes.Text,

        controlType:
          ControlTypes.Text,

        IsEditable: true,

        IsFilterable: true,

        IsVisible: true,

        DefaultValue: '',

        Width: 320,

        DomID: 'Description'

      } as IColumns,

      {
        Name: 'price',

        DisplayName: 'Price',

        DataType:
          DataTypes.NUMERIC,

        controlType:
          ControlTypes.Number,

        IsEditable: true,

        IsFilterable: true,

        IsVisible: true,

        DefaultValue: 0,

        Width: 130,

        DomID: 'Price'

      } as IColumns,

      {
        Name: 'quantity',

        DisplayName: 'Quantity',

        DataType:
          DataTypes.NUMERIC,

        controlType:
          ControlTypes.Number,

        IsEditable: true,

        IsFilterable: true,

        IsVisible: true,

        DefaultValue: 0,

        Width: 130,

        DomID: 'Quantity'

      } as IColumns
    ];

  searchValue = '';

  constructor(
    public readonly dataSource:
      ProductsDataSource,
    private readonly auth: AuthService,
    private readonly productsService: ProductsService
  ) {}

  get isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  // Template helper: returns the changeSet or null typed as any so the template
  // can pass null to BI-Grid to disable edit UI for non-admins without
  // triggering Angular's strict template type checks.
  get changeSetBinding(): any {
    return this.isAdmin ? this.changeSet : null;
  }

ngOnInit(): void {
  this.dataSource.read();
}

searchProducts(
  event: Event
): void {
  const input =
    event.target as HTMLInputElement;

  this.searchValue =
    input.value;

  this.dataSource.read();
}

clearSearch(): void {
  this.searchValue = '';
  this.dataSource.read();
}
  refresh(): void {

    this.grid?.read();
  }

// -----------------------------------------
// Admin actions: prompt-based quick flows
// -----------------------------------------

addProductPrompt(): void {
  const name = window.prompt('Product name:');
  if (!name) { return; }

  const description = window.prompt('Product description:') ?? '';
  const priceStr = window.prompt('Price (numeric):', '0') ?? '0';
  const quantityStr = window.prompt('Quantity (integer):', '0') ?? '0';

  const price = parseFloat(priceStr) || 0;
  const quantity = parseInt(quantityStr, 10) || 0;

  this.productsService.createProduct({
    name: name.trim(),
    description: description.trim(),
    price,
    quantity
  }).subscribe({
    next: () => {
      this.grid?.read();
      alert('Product created');
    },
    error: (err) => {
      console.error(err);
      alert('Failed to create product');
    }
  });
}

editProductPrompt(): void {
  const idStr = window.prompt('Enter product ID to edit:');
  const id = idStr ? parseInt(idStr, 10) : NaN;
  if (!id || isNaN(id)) { return; }

  this.productsService.getProduct(id).subscribe({
    next: (product) => {
      const name = window.prompt('Product name:', product.name) ?? product.name;
      const description = window.prompt('Product description:', product.description) ?? product.description;
      const priceStr = window.prompt('Price (numeric):', String(product.price)) ?? String(product.price);
      const quantityStr = window.prompt('Quantity (integer):', String(product.quantity)) ?? String(product.quantity);

      const price = parseFloat(priceStr) || 0;
      const quantity = parseInt(quantityStr, 10) || 0;

      this.productsService.updateProduct(id, {
        name: name.trim(),
        description: description.trim(),
        price,
        quantity
      }).subscribe({
        next: () => {
          this.grid?.read();
          alert('Product updated');
        },
        error: (err) => {
          console.error(err);
          alert('Failed to update product');
        }
      });
    },
    error: (err) => {
      console.error(err);
      alert('Product not found or error reading product');
    }
  });
}

deleteProductPrompt(): void {
  const idStr = window.prompt('Enter product ID to delete:');
  const id = idStr ? parseInt(idStr, 10) : NaN;
  if (!id || isNaN(id)) { return; }

  if (!confirm(`Delete product id ${id}? This cannot be undone.`)) { return; }

  this.productsService.deleteProduct(id).subscribe({
    next: () => {
      this.grid?.read();
      alert('Product deleted');
    },
    error: (err) => {
      console.error(err);
      alert('Failed to delete product');
    }
  });
}
}
