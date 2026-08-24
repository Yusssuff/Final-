export interface Product {
  id: number;
  name: string;
  price: number;
  quantity: number;
}

export interface CreateProductRequest {
  name: string;
  price: number;
  quantity: number;
}
