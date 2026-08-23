import { Routes } from '@angular/router';
import { ProductsComponent } from './products/products.compoent';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full',
  },
  {
    path: 'products',
    component: ProductsComponent,
  },
];
