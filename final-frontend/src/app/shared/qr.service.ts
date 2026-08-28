import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class QrService {
  // Lazy import so qrcode isn't in the initial bundle
  async toDataUrl(text: string, options?: any): Promise<string> {
    // @ts-ignore: dynamic import of qrcode (typed at runtime)
    const QRCode = await import('qrcode');
    const opts = Object.assign({
      width: 300,
      errorCorrectionLevel: 'M',
      color: {
        dark: '#0f172a',
        light: '#ffffff',
      },
    }, options);
    return QRCode.toDataURL(text, opts);
  }

  async toSvg(text: string, options?: any): Promise<string> {
    // @ts-ignore: dynamic import of qrcode (typed at runtime)
    const QRCode = await import('qrcode');
    const opts = Object.assign({
      type: 'svg',
      errorCorrectionLevel: 'M',
      color: {
        dark: '#ffffff',
        light: '#0000',
      },
    }, options);
    return QRCode.toString(text, opts);
  }
}
