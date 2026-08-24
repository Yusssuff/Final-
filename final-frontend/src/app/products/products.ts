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

@Component({
  selector: 'app-products',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    BIModulesModule
  ],

  templateUrl: './products.html',

styleUrl: './products.css',

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
      ProductsDataSource
  ) {}

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
}
