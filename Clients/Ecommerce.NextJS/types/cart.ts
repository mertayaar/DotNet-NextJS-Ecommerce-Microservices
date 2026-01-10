export interface CartItemDto {
    productId: string;
    productName: string;
    productImageUrl: string;
    quantity: number;
    price: number;
    categoryId?: string;
}

export interface CartTotalDto {
    userId?: string; 
    discountCode?: string;
    discountRate?: number;
    cartItems: CartItemDto[];
    totalPrice: number;
}
