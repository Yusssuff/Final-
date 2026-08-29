import { Injectable, inject } from '@angular/core';

import { BehaviorSubject, Observable, forkJoin } from 'rxjs';

import { ControlTypes, DataTypes, IDataSource } from 'bi-interfaces';

import { PublicApiClient } from '@salesbuzz/public-sdk';

import { Product } from './products.model';

export interface GridResult<T> {
  data: T[];
  total: number;
}

@Injectable({
  providedIn: 'root',
})
export class ProductsDataSource
  extends BehaviorSubject<GridResult<Product>>
  implements IDataSource
{
  private readonly api = inject(PublicApiClient);

  Params: IDataSource['Params'] = [];

  Key = 'id';

  Key2 = '';
  Key3 = '';
  Key4 = '';
  Key5 = '';
  Key6 = '';

  Columns: IDataSource['Columns'] = [
    {
      Name: 'id',
      DataType: DataTypes.NUMERIC,
      controlType: ControlTypes.Number,
    },
    {
      Name: 'name',
      DataType: DataTypes.Text,
      Length: 150,
      controlType: ControlTypes.Text,
    },
    {
      Name: 'price',
      DataType: DataTypes.NUMERIC,
      controlType: ControlTypes.Number,
    },
    {
      Name: 'quantity',
      DataType: DataTypes.NUMERIC,
      controlType: ControlTypes.Number,
    },
  ];


  Type = 'api' as IDataSource['Type'];

  IsClientSideFilter = true;

  LocalData = false;

  HasPaging = true;

  data: Product[] = [];

  state = {
    skip: 0,
    take: 10,
    sort: [] as [],
  };

  /*
   * Keep this as a valid non-null string.
   */
  APIURL = 'api/Products';

  POSTAPIURL = 'api/Products';

  PUTAPIURL = 'api/Products';

  DELETEAPIURL = 'api/Products';

  excludeDataFromReq: string[] = [];

  excludeTimeFromReq: string[] = [];

  loading = false;

  constructor() {
    super({
      data: [],
      total: 0,
    });
  }

  read(filter: string = ''): void {
    this.loading = true;

    this.api.get<Product[]>(this.APIURL).subscribe({
      next: (products) => {
        this.data = Array.isArray(products) ? products : [];

        // Determine if the provided filter is a query-string (paging/filter from BI-Grid)
        const isQueryString = /[=&?]/.test(filter);

        let source = this.data;

        // If the caller passed a plain search term (no '=' or '&'), perform a client-side
        // text search over name and description fields to support the app search box.
        if (filter && !isQueryString) {
          const q = filter.trim().toLowerCase();
          if (q.length > 0) {
            source = this.data.filter((p) => {
              const name = (p.name || '').toString().toLowerCase();
              const idStr = (p.id || '').toString();

              return name.includes(q) || idStr === q;
            });
          }
        }

        const skip = this.getSkip(filter);
        const take = this.getTake(filter);

        const total = source.length;

        const page = source.slice(skip, skip + take);

        this.state.skip = skip;

        this.state.take = take;

        this.next({
          data: page,
          total,
        });

        this.loading = false;
      },

      error: (error) => {
        console.error('Failed to load Products:', error);

        this.data = [];

        this.next({
          data: [],
          total: 0,
        });

        this.loading = false;
      },
    });
  }

  get(APIURL: string): Observable<any> {
    return this.api.get<any>(APIURL);
  }

  add(data: any): Observable<any> {
    return this.api.post<any>(this.POSTAPIURL, data);
  }

  edit(data: any, id: string): Observable<any> {
    return this.api.put<any>(
      `${this.PUTAPIURL}/${encodeURIComponent(id)}`,
      data,
    );
  }

  patch(data: any, id: string): Observable<any> {
    return this.api.patch<any>(
      `${this.PUTAPIURL}/${encodeURIComponent(id)}`,
      data,
    );
  }

  delete(id: string): Observable<any> {
    return this.api.delete<any>(
      `${this.DELETEAPIURL}/${encodeURIComponent(id)}`,
    );
  }

  batch(
    CreatedItemArray: any[],
    UpdatedItemArray: any[],
    DeletedItemArray: any[],
  ): Observable<any> {
    /*
     * Implement batch by issuing the necessary API calls for
     * created, updated, and deleted items. Use forkJoin to run
     * them in parallel and return a combined result. This allows
     * the BI-Grid/BI-Nav Save flow to persist changes to the backend.
     */
    const calls: Observable<any>[] = [];

    // Created items -> POST
    if (Array.isArray(CreatedItemArray) && CreatedItemArray.length > 0) {
      for (const item of CreatedItemArray) {
        calls.push(this.add(item));
      }
    }

    // Updated items -> PUT (edit)
    if (Array.isArray(UpdatedItemArray) && UpdatedItemArray.length > 0) {
      for (const upd of UpdatedItemArray) {
        // BI Grid/changeset may provide { id, data } or similar; try common shapes
        const id = (upd && (upd.id ?? upd.key ?? upd.Key))?.toString();
        const payload = upd && (upd.data ?? upd.payload ?? upd);
        if (id) {
          calls.push(this.edit(payload, id));
        } else if (upd && typeof upd === 'object') {
          // fallback: try to call put on item.id
          const fallbackId = (upd as any).id;
          if (fallbackId !== undefined) {
            calls.push(this.edit(payload || upd, fallbackId.toString()));
          }
        }
      }
    }

    // Deleted items -> DELETE
    if (Array.isArray(DeletedItemArray) && DeletedItemArray.length > 0) {
      for (const del of DeletedItemArray) {
        const id = (del && (del.id ?? del.key ?? del.Key ?? del))?.toString();
        if (id) {
          calls.push(this.delete(id));
        }
      }
    }

    if (calls.length === 0) {
      // Nothing to do
      return new Observable((observer) => {
        observer.next([]);
        observer.complete();
      });
    }

    // Run all calls in parallel and return their responses as an array
    return forkJoin(calls);
  }

  formatAPIURLWithFilter(filter: string): string {
    const raw = (filter ?? '').replace(/^&|^\?/, '');

    if (!raw) {
      return this.APIURL;
    }

    // Ensure the APIURL and filter are joined correctly with ? or &
    const sep = this.APIURL.includes('?') ? '&' : '?';
    return `${this.APIURL}${sep}${raw}`;
  }

  formatFilter(filter: string): string {
    return filter ?? '';
  }

  private getSkip(filter: string): number {
    if (!filter) {
      return 0;
    }

    const params = new URLSearchParams(filter.replace(/^&/, ''));

    const value = Number(params.get('$skip'));

    if (!Number.isFinite(value) || value < 0) {
      return 0;
    }

    return Math.floor(value);
  }

  private getTake(filter: string): number {
    if (!filter) {
      return 10;
    }

    const params = new URLSearchParams(filter.replace(/^&/, ''));

    const value = Number(params.get('$top'));

    if (!Number.isFinite(value) || value <= 0) {
      return 10;
    }

    return Math.floor(value);
  }
}
