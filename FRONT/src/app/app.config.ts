import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import {
  provideHttpClient,
  withFetch,
} from '@angular/common/http';
import { provideRouter } from '@angular/router';

import {
  PublicApiClient,
  PublicSdkModule,
} from '@salesbuzz/public-sdk';

import { AppPublicApiClient } from './public-api-client';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    provideHttpClient(withFetch()),

    importProvidersFrom(PublicSdkModule),

    {
      provide: PublicApiClient,
      useExisting: AppPublicApiClient,
    },
  ],
};
