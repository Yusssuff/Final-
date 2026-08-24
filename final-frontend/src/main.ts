import '@angular/localize/init';
import {
  platformBrowserDynamic
} from '@angular/platform-browser-dynamic';

import {
  AppModule
} from './app/app.module';

if (
  typeof localStorage !== 'undefined' &&
  !localStorage.getItem('lang')
) {
  localStorage.setItem('lang', 'en-US');
}

if (
  typeof localStorage !== 'undefined' &&
  !localStorage.getItem('isRightToLeft')
) {
  localStorage.setItem(
    'isRightToLeft',
    'ltr'
  );
}

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch(error => {
    console.error(error);
  });
