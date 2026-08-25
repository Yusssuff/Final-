import {
  Routes
} from '@angular/router';

import {
  AuthComponent
} from './auth/auth.component';

import {
  authGuard
} from './auth/auth.guard';

import {
  OrderDetailComponent
} from './orders/order-detail';

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
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./profile/profile.component').then(m => m.ProfileComponent)
  },

  {
    path: 'order-details',
    canActivate: [authGuard],
    component: OrderDetailComponent
  },

  {
    path: '**',
    redirectTo: 'auth'
  }
];
