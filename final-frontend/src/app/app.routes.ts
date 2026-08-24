import {
  Routes
} from '@angular/router';

import {
  AuthComponent
} from './auth/auth.component';

import {
  authGuard
} from './auth/auth.guard';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'auth',
    pathMatch: 'full'
  },

  {
    path: 'auth',
    component: AuthComponent
  },

  {
    path: 'products',
    canActivate: [authGuard],

    loadComponent: () =>
      import('./products/products')
        .then(
          m => m.Products
        )
  },

  {
    path: '**',
    redirectTo: 'auth'
  }
];
