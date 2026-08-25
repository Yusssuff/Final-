export interface Order {
  id: number;
  userId: number;
  productId: number;
  quantity: number;
  totalPrice: number;
  orderDate: string;
  product?: {
    id: number;
    name: string;
    price: number;
    quantity: number;
  };
}

export interface CreateOrderRequest {
  productId: number;
  quantity: number;
}
