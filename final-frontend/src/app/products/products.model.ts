export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  quantity: number;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  quantity: number;
}
