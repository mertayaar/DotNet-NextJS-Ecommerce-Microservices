import { CartTotalDto } from '@/types/cart';
import { Product } from '@/types/product';
import { authenticatedFetch } from './api';

export async function getMyCart(): Promise<CartTotalDto | null> {
    try {
        const response = await authenticatedFetch('/api/cart/carts', {
            method: 'GET'
        });

        if (response.status === 204) {
            return null; 
        }

        if (!response.ok) {
            console.error('Failed to fetch cart:', response.status);
            return null;
        }

        const result = await response.json();
        return result.data || result;
    } catch (error) {
        console.error('Error fetching cart:', error);
        return null;
    }
}

export async function saveCart(cartTotal: CartTotalDto): Promise<boolean> {
    try {
        
        
        const { userId, totalPrice, ...cartPayload } = cartTotal;

        const response = await authenticatedFetch('/api/cart/carts', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cartPayload)
        });

        return response.ok;
    } catch (error) {
        console.error('Error saving cart:', error);
        return false;
    }
}

export async function deleteCart(): Promise<boolean> {
    try {
        const response = await authenticatedFetch('/api/cart/carts', {
            method: 'DELETE'
        });

        return response.ok;
    } catch (error) {
        console.error('Error deleting cart:', error);
        return false;
    }
}

export interface ApplyCouponResult {
    success: boolean;
    message: string;
    discountRate?: number;
}

export async function applyCoupon(couponCode: string): Promise<ApplyCouponResult> {
    try {
        const response = await authenticatedFetch('/api/cart/carts/apply-coupon', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ couponCode })
        });

        const result = await response.json();

        if (!response.ok) {
            return {
                success: false,
                message: result.message || 'Failed to apply coupon'
            };
        }

        return {
            success: true,
            message: result.data?.message || 'Coupon applied!',
            discountRate: result.data?.discountRate
        };
    } catch (error) {
        console.error('Error applying coupon:', error);
        return {
            success: false,
            message: 'Failed to apply coupon'
        };
    }
}

export async function removeCoupon(): Promise<boolean> {
    try {
        const response = await authenticatedFetch('/api/cart/carts/remove-coupon', {
            method: 'DELETE'
        });

        return response.ok;
    } catch (error) {
        console.error('Error removing coupon:', error);
        return false;
    }
}

export async function addToCart(product: Product): Promise<boolean> {
    let cart = await getMyCart();

    if (!cart) {
        cart = {
            discountCode: "",
            cartItems: [],
            totalPrice: 0
        };
    }

    const existingItem = cart.cartItems.find(x => x.productId === product.productId);
    if (existingItem) {
        existingItem.quantity += 1;
    } else {
        cart.cartItems.push({
            productId: product.productId,
            productName: product.productName,
            price: product.productPrice,
            productImageUrl: product.productImageUrl,
            quantity: 1,
            categoryId: product.categoryId
        });
    }

    cart.totalPrice = cart.cartItems.reduce((acc, item) => acc + (item.price * item.quantity), 0);

    return await saveCart(cart);
}
