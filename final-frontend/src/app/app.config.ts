import { ApplicationConfig, importProvidersFrom } from '@angular/core';

import { provideRouter } from '@angular/router';

import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { TranslateModule } from '@ngx-translate/core';

import {
  CreateDialog,
  PublicApiClient,
  PublicSdkModule,
} from '@salesbuzz/public-sdk';

import { NavInfo } from 'bi-interfaces';

import { routes } from './app.routes';

import { authInterceptor } from './auth/auth.interceptor';

import { SalesBuzzApiClient } from './sdk-api-client';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    provideHttpClient(withInterceptors([authInterceptor])),

    importProvidersFrom(
      BrowserAnimationsModule,
      TranslateModule.forRoot(),
      PublicSdkModule,
    ),

    {
      provide: PublicApiClient,
      useClass: SalesBuzzApiClient,
    },

    {
      provide: 'CreateDialog',
      useFactory: () => {
        const dialog = new CreateDialog();

        return () => dialog;
      },
    },

    {
      provide: NavInfo,
      useValue: {
        getBUDesc: (buid: string) => buid,
      },
    },
  ],
};
