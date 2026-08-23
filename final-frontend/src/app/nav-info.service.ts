import { Injectable } from '@angular/core';

import {
  NavInfo
} from '@salesbuzz/public-sdk';

@Injectable({
  providedIn: 'root'
})
export class SalesBuzzNavInfoService
  extends NavInfo {

  override getBUDesc(
    BUID: string
  ): any {
    return BUID;
  }
}
