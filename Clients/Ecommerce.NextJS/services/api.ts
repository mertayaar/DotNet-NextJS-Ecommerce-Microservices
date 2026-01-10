

import { Product } from '@/types/product';

const BFF_URL = process.env.NEXT_PUBLIC_BFF_URL || '/backend';

export interface CategoryDto {
    categoryId: string;
    categoryName: string;
}






export async function authenticatedFetch(
    endpoint: string,
    options: RequestInit = {}
): Promise<Response> {
    const url = `${BFF_URL}${endpoint}`;

    let response = await fetch(url, {
        ...options,
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
            ...options.headers,
        },
    });

    if (response.status === 401) {
        try {
            const refreshResponse = await fetch(`${BFF_URL}/api/auth/refresh`, {
                method: 'POST',
                credentials: 'include',
            });

            if (refreshResponse.ok) {
                response = await fetch(url, {
                    ...options,
                    credentials: 'include',
                    headers: {
                        'Content-Type': 'application/json',
                        ...options.headers,
                    },
                });
            }
        } catch (error) {
            console.error('Token refresh failed', error);
        }
    }

    return response;
}


export async function publicFetch(
    endpoint: string,
    options: RequestInit = {}
): Promise<Response> {
    const url = `${BFF_URL}${endpoint}`;

    const response = await fetch(url, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...options.headers,
        },
    });

    return response;
}





export async function getProducts(): Promise<Product[]> {
    const response = await publicFetch('/api/product/products');

    if (!response.ok) {
        return [];
    }

    const result = await response.json();
    return result.data || result || [];
}

export async function getProduct(id: string): Promise<Product | null> {
    const response = await publicFetch(`/api/product/products/${id}`);

    if (!response.ok) {
        return null;
    }

    const result = await response.json();
    return result.data || result;
}

export async function getProductBySlug(slug: string): Promise<Product | null> {
    const response = await publicFetch(`/api/product/products/slug/${slug}`);

    if (!response.ok) {
        return null;
    }

    const result = await response.json();
    return result.data || result;
}

export async function getProductsByCategory(categoryId: string): Promise<Product[]> {
    const response = await publicFetch(`/api/product/products/category/${categoryId}`);

    if (!response.ok) {
        return [];
    }

    const result = await response.json();
    return result.data || result || [];
}

export async function searchProducts(query: string): Promise<Product[]> {
    const response = await publicFetch(`/api/product/products/search?query=${encodeURIComponent(query)}`);

    if (!response.ok) {
        return [];
    }

    const result = await response.json();
    return result.data || result || [];
}

export async function fetchCategories(): Promise<CategoryDto[]> {
    const response = await publicFetch('/api/product/categories');

    if (!response.ok) {
        return [];
    }

    const result = await response.json();
    return result.data || result || [];
}

export async function getCategory(id: string): Promise<CategoryDto | null> {
    const response = await publicFetch(`/api/product/categories/${id}`);

    if (!response.ok) {
        return null;
    }

    const result = await response.json();
    return result.data || result;
}





export interface BasketItemDto {
    productId: string;
    productName: string;
    price: number;
    quantity: number;
    productImageUrl: string;
}

export interface BasketDto {
    userId: string;
    discountCode?: string;
    discountRate?: number;
    totalPrice: number;
    basketItems: BasketItemDto[];
}

export async function getBasket(): Promise<BasketDto | null> {
    const response = await authenticatedFetch('/api/cart/carts');

    if (!response.ok) {
        return null;
    }

    const result = await response.json();
    return result.data || result;
}





export interface CreateCargoCustomerDto {
    name: string;
    surname: string;
    email: string;
    phone: string;
    district: string;
    city: string;
    address: string;
    userCustomerId?: string;
}

export async function createCargoCustomer(data: CreateCargoCustomerDto): Promise<boolean> {
    const response = await authenticatedFetch('/api/cargo/cargocustomers', {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.ok;
}





export interface CreateOrderDto {
    userId: string;
    totalPrice: number;
    orderDate: string;
    discountCode?: string;
    discountRate?: number;
    orderItems: {
        productId: string;
        productName: string;
        productPrice: number;
        productAmount: number;
        productTotalPrice: number;
        productImageUrl: string;
    }[];
}

export async function createOrder(data: CreateOrderDto): Promise<boolean> {
    const response = await authenticatedFetch('/api/order/orderings', {
        method: 'POST',
        body: JSON.stringify(data),
    });

    return response.ok;
}

export interface OrderingDto {
    orderingId: number;
    userId: string;
    totalPrice: number;
    orderDate: string;
    discountCode?: string;
    discountRate?: number;
}

export async function getOrderingByUserId(userId: string): Promise<OrderingDto[]> {
    const response = await authenticatedFetch(`/api/order/orderings/GetOrderingByUserId/${userId}`);

    if (response.status === 404 || response.status === 204) {
        return [];
    }

    if (!response.ok) {
        console.error("Failed to fetch orders");
        return [];
    }

    const result = await response.json();
    return result.data || result || [];
}

export async function getOrderingById(orderingId: string): Promise<OrderingDto | null> {
    const response = await authenticatedFetch(`/api/order/orderings/${orderingId}`);

    if (response.status === 404 || response.status === 204) {
        return null;
    }

    if (!response.ok) {
        return null;
    }

    const result = await response.json();
    return result.data || result;
}

export interface OrderDetailDto {
    orderDetailId: number;
    productId: string;
    productName: string;
    productPrice: number;
    productAmount: number;
    productTotalPrice: number;
    orderingId: number;
    productImageUrl: string;
}

export async function getOrderDetails(orderingId: string): Promise<OrderDetailDto[]> {
    const response = await authenticatedFetch(`/api/order/OrderDetails/GetOrderDetailByOrderingId/${orderingId}`);

    if (response.status === 404 || response.status === 204) {
        return [];
    }

    if (!response.ok) {
        console.error("Failed to fetch order details");
        return [];
    }

    const result = await response.json();
    return result.data || result || [];
}
