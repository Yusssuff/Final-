import {
  Component,
  OnInit,
  inject
} from '@angular/core';

import {
  CommonModule
} from '@angular/common';

import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  Validators
} from '@angular/forms';

import {
  ProductsService
} from './products.service';

import {
  Product,
  CreateProductRequest
} from './products.model';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products
  implements OnInit {

  private readonly productsService =
    inject(ProductsService);

  private readonly fb =
    inject(FormBuilder);

  // -----------------------------------------
  // Products
  // -----------------------------------------

  products: Product[] = [];

  searchValue = '';

  // -----------------------------------------
  // UI state
  // -----------------------------------------

  isLoading = false;

  isSaving = false;

  isModalOpen = false;

  errorMessage = '';

  successMessage = '';

  // -----------------------------------------
  // Product form
  // -----------------------------------------

  readonly productForm =
    this.fb.nonNullable.group({

      name: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(150)
        ]
      ],

      description: [
        '',
        [
          Validators.maxLength(500)
        ]
      ],

      price: [
        0,
        [
          Validators.required,
          Validators.min(0)
        ]
      ],

      quantity: [
        0,
        [
          Validators.required,
          Validators.min(0)
        ]
      ]

    });

  // -----------------------------------------
  // Filtered products
  // -----------------------------------------

  get filteredProducts(): Product[] {

    const query =
      this.searchValue
        .trim()
        .toLowerCase();

    if (!query) {
      return this.products;
    }

    return this.products.filter(
      (product) => {

        const id =
          product.id.toString();

        const name =
          product.name
            ?.toLowerCase() ?? '';

        const description =
          product.description
            ?.toLowerCase() ?? '';

        return (
          id.includes(query) ||
          name.includes(query) ||
          description.includes(query)
        );
      }
    );
  }

  // -----------------------------------------
  // INIT
  // -----------------------------------------

  ngOnInit(): void {
    this.loadProducts();
  }

  // -----------------------------------------
  // LOAD PRODUCTS
  // -----------------------------------------

  loadProducts(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.productsService
      .getProducts()
      .subscribe({

        next: (products) => {

          this.products =
            products ?? [];

          this.isLoading = false;
        },

        error: (error: unknown) => {

          console.error(
            'Products API error:',
            error
          );

          this.isLoading = false;

          this.errorMessage =
            this.getErrorMessage(
              error,
              'Unable to load products.'
            );
        }

      });
  }

  // -----------------------------------------
  // SEARCH
  // -----------------------------------------

  clearSearch(): void {
    this.searchValue = '';
  }

  // -----------------------------------------
  // CREATE MODAL
  // -----------------------------------------

  openCreate(): void {

    this.errorMessage = '';

    this.successMessage = '';

    this.productForm.reset({
      name: '',
      description: '',
      price: 0,
      quantity: 0
    });

    this.isModalOpen = true;
  }

  // -----------------------------------------
  // CLOSE MODAL
  // -----------------------------------------

  closeModal(): void {

    if (this.isSaving) {
      return;
    }

    this.isModalOpen = false;

    this.productForm.reset({
      name: '',
      description: '',
      price: 0,
      quantity: 0
    });
  }

  // -----------------------------------------
  // CREATE PRODUCT
  // -----------------------------------------

  createProduct(): void {

    if (this.productForm.invalid) {

      this.productForm.markAllAsTouched();

      return;
    }

    this.isSaving = true;

    this.errorMessage = '';

    this.successMessage = '';

    const value =
      this.productForm.getRawValue();

    const request:
      CreateProductRequest = {

      name:
        value.name.trim(),

      description:
        value.description.trim(),

      price:
        Number(value.price),

      quantity:
        Number(value.quantity)

    };

    this.productsService
      .createProduct(request)
      .subscribe({

        next: (product) => {

          this.isSaving = false;

          this.isModalOpen = false;

          this.successMessage =
            'Product created successfully.';

          if (product) {

            this.products = [
              product,
              ...this.products
            ];

          } else {

            this.loadProducts();

          }

          this.productForm.reset({
            name: '',
            description: '',
            price: 0,
            quantity: 0
          });
        },

        error: (error: unknown) => {

          console.error(
            'Create product error:',
            error
          );

          this.isSaving = false;

          this.errorMessage =
            this.getErrorMessage(
              error,
              'Unable to create product.'
            );
        }

      });
  }

  // -----------------------------------------
  // ERROR HANDLING
  // -----------------------------------------

  private getErrorMessage(
    error: unknown,
    fallback: string
  ): string {

    if (
      typeof error !== 'object' ||
      error === null
    ) {
      return fallback;
    }

    const response =
      error as {
        error?: unknown;
        message?: unknown;
      };

    if (
      typeof response.message ===
      'string'
    ) {
      return response.message;
    }

    if (
      typeof response.error ===
      'string'
    ) {
      return response.error;
    }

    if (
      typeof response.error ===
      'object' &&
      response.error !== null
    ) {

      const nested =
        response.error as {
          message?: unknown;
          title?: unknown;
        };

      if (
        typeof nested.message ===
        'string'
      ) {
        return nested.message;
      }

      if (
        typeof nested.title ===
        'string'
      ) {
        return nested.title;
      }
    }

    return fallback;
  }
}
