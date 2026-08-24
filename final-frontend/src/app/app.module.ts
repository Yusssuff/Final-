import { NgModule } from '@angular/core';

import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { RouterModule } from '@angular/router';

import { TranslateModule } from '@ngx-translate/core';

import {
  CreateDialog,
  PublicApiClient,
  PublicSdkModule
} from '@salesbuzz/public-sdk';

import { NavInfo } from 'bi-interfaces';

import { of } from 'rxjs';

import { AppComponent } from './app';
import { AuthComponent } from './auth/auth.component';

import { routes } from './app.routes';

import { SalesBuzzApiClient } from './sdk-api-client';

import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';

import { authInterceptor } from './auth/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent
  ],

  imports: [
    BrowserModule,

    BrowserAnimationsModule,

    HttpClientModule,

    ReactiveFormsModule,

    RouterModule.forRoot(routes),

    TranslateModule.forRoot(),

    PublicSdkModule,

    AuthComponent
  ],

  providers: [

    provideHttpClient(
      withInterceptors([
        authInterceptor
      ])
    ),

    {
      provide: PublicApiClient,
      useClass: SalesBuzzApiClient
    },

    {
      provide: 'CreateDialog',

      useFactory: () => {
        const dialog = new CreateDialog();

        return () => dialog;
      }
    },

    {
      provide: NavInfo,

      useValue: {
        getBUDesc: (buid: string) => of(buid)
      }
    }
  ],

  bootstrap: [
    AppComponent
  ]
})
export class AppModule {}
