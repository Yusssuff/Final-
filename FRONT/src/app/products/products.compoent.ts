import { CurrencyPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import {
  CreateProductRequest,
  Product,
  UpdateProductRequest,
} from './products.model';
import { ProductService } from './products.serv';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CurrencyPipe,
    ReactiveFormsModule,
  ],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class ProductsComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly productService = inject(ProductService);

  protected readonly products = signal<Product[]>([]);
  protected readonly searchTerm = signal('');

  protected readonly isLoading = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly deletingId = signal<number | null>(null);

  protected readonly errorMessage = signal('');
  protected readonly feedbackMessage = signal('');

  protected readonly isFormOpen = signal(false);
  protected readonly editingProduct = signal<Product | null>(null);

  protected readonly filteredProducts = computed(() => {
    const term = this.searchTerm()
      .trim()
      .toLowerCase();

    const products = this.products();

    if (!term) {
      return products;
    }

    return products.filter((product) =>
      product.name
        .toLowerCase()
        .includes(term),
    );
  });

  protected readonly productForm =
    this.formBuilder.nonNullable.group({
      name: [
        '',
        [
          Validators.required,
          Validators.maxLength(150),
        ],
      ],
      price: [
        0,
        [
          Validators.required,
          Validators.min(0),
        ],
      ],
      quantity: [
        0,
        [
          Validators.required,
          Validators.min(0),
        ],
      ],
    });

  ngOnInit(): void {
    this.loadProducts();
  }

  protected loadProducts(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(
          this.getErrorMessage(
            error,
            'Unable to load products.',
          ),
        );

        this.isLoading.set(false);
      },
    });
  }

  protected updateSearch(term: string): void {
    this.searchTerm.set(term);
  }

  protected openCreateForm(): void {
    this.editingProduct.set(null);

    this.productForm.reset({
      name: '',
      price: 0,
      quantity: 0,
    });

    this.productForm.markAsUntouched();
    this.productForm.markAsPristine();

    this.errorMessage.set('');
    this.feedbackMessage.set('');

    this.isFormOpen.set(true);
  }

  protected openEditForm(
    product: Product,
  ): void {
    this.editingProduct.set(product);

    this.productForm.setValue({
      name: product.name,
      price: product.price,
      quantity: product.quantity,
    });

    this.productForm.markAsUntouched();
    this.productForm.markAsPristine();

    this.errorMessage.set('');
    this.feedbackMessage.set('');

    this.isFormOpen.set(true);
  }

  protected closeForm(): void {
    if (this.isSaving()) {
      return;
    }

    this.isFormOpen.set(false);
    this.editingProduct.set(null);
  }

  protected saveProduct(): void {
    if (this.isSaving()) {
      return;
    }

    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    const value =
      this.productForm.getRawValue();

    const request:
      | CreateProductRequest
      | UpdateProductRequest = {
      name: value.name.trim(),
      price: value.price,
      quantity: value.quantity,
    };

    if (!request.name) {
      this.productForm.controls.name.setErrors({
        required: true,
      });

      this.productForm.controls.name.markAsTouched();

      return;
    }

    this.isSaving.set(true);
    this.feedbackMessage.set('');
    this.errorMessage.set('');

    const editingProduct =
      this.editingProduct();

    if (editingProduct) {
      this.productService
        .updateProduct(
          editingProduct.id,
          request as UpdateProductRequest,
        )
        .subscribe({
          next: () => {
            this.isSaving.set(false);
            this.isFormOpen.set(false);
            this.editingProduct.set(null);

            this.feedbackMessage.set(
              'Product updated successfully.',
            );

            this.loadProducts();
          },

          error: (error: HttpErrorResponse) => {
            this.isSaving.set(false);

            this.feedbackMessage.set(
              this.getErrorMessage(
                error,
                'Unable to update the product.',
              ),
            );
          },
        });

      return;
    }

    this.productService
      .createProduct(
        request as CreateProductRequest,
      )
      .subscribe({
        next: () => {
          this.isSaving.set(false);
          this.isFormOpen.set(false);

          this.feedbackMessage.set(
            'Product created successfully.',
          );

          this.loadProducts();
        },

        error: (error: HttpErrorResponse) => {
          this.isSaving.set(false);

          this.feedbackMessage.set(
            this.getErrorMessage(
              error,
              'Unable to create the product.',
            ),
          );
        },
      });
  }

  protected deleteProduct(
    product: Product,
  ): void {
    if (this.deletingId() !== null) {
      return;
    }

    const confirmed =
      window.confirm(
        `Delete "${product.name}"?`,
      );

    if (!confirmed) {
      return;
    }

    this.deletingId.set(product.id);
    this.feedbackMessage.set('');
    this.errorMessage.set('');

    this.productService
      .deleteProduct(product.id)
      .subscribe({
        next: () => {
          this.deletingId.set(null);

          this.feedbackMessage.set(
            'Product deleted successfully.',
          );

          this.loadProducts();
        },

        error: (error: HttpErrorResponse) => {
          this.deletingId.set(null);

          this.feedbackMessage.set(
            this.getErrorMessage(
              error,
              'Unable to delete the product.',
            ),
          );
        },
      });
  }

  protected quantityClass(
    quantity: number,
  ): string {
    if (quantity === 0) {
      return 'bg-red-50 text-red-700 ring-red-600/20';
    }

    if (quantity < 10) {
      return 'bg-amber-50 text-amber-700 ring-amber-600/20';
    }

    return 'bg-emerald-50 text-emerald-700 ring-emerald-600/20';
  }

  private getErrorMessage(
    error: HttpErrorResponse,
    fallback: string,
  ): string {
    const message =
      typeof error.error?.message === 'string'
        ? error.error.message
        : '';

    if (message) {
      return message;
    }

    if (error.status === 400) {
      return 'Invalid product data.';
    }

    if (error.status === 401) {
      return 'You must be logged in.';
    }

    if (error.status === 403) {
      return 'You do not have permission for this action.';
    }

    if (error.status === 404) {
      return 'Product not found.';
    }

    if (error.status === 0) {
      return 'Cannot connect to the backend at http://localhost:5125.';
    }

    return fallback;
  }
}

