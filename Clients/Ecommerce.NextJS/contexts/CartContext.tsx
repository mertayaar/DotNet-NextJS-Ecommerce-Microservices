"use client";



import React, { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';
import { CartTotalDto, CartItemDto } from '@/types/cart';
import { Product } from '@/types/product';
import { getMyCart, saveCart, deleteCart } from '@/services/cartService';
import { useAuth } from '@/contexts/AuthContext';





interface CartContextType {
    
    cart: CartTotalDto | null;
    
    itemCount: number;
    
    isLoading: boolean;
    
    isSaving: boolean;
    
    error: string | null;
    
    addToCart: (product: Product, quantity?: number) => Promise<boolean>;
    
    removeFromCart: (productId: string) => Promise<boolean>;
    
    updateQuantity: (productId: string, quantity: number) => Promise<boolean>;
    
    clearCart: () => Promise<boolean>;
    
    refreshCart: () => Promise<void>;
    
    clearError: () => void;
}





const CartContext = createContext<CartContextType | undefined>(undefined);






const SAVE_DEBOUNCE_MS = 500;





interface CartProviderProps {
    children: React.ReactNode;
}

export function CartProvider({ children }: CartProviderProps) {
    
    
    

    const [cart, setCart] = useState<CartTotalDto | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const { isAuthenticated, isLoading: authLoading } = useAuth();

    const saveTimeoutRef = useRef<NodeJS.Timeout | null>(null);
    const isMountedRef = useRef(true);

    
    
    

    
    const itemCount = cart?.cartItems?.reduce((total, item) => total + item.quantity, 0) || 0;

    
    
    

    
    const refreshCart = useCallback(async () => {
        if (!isAuthenticated) {
            setCart(null);
            return;
        }

        setIsLoading(true);
        setError(null);

        try {
            const cartData = await getMyCart();
            if (isMountedRef.current) {
                setCart(cartData);
            }
        } catch (err) {
            console.error('Error fetching cart:', err);
            if (isMountedRef.current) {
                setError('Failed to load cart');
                setCart(null);
            }
        } finally {
            if (isMountedRef.current) {
                setIsLoading(false);
            }
        }
    }, [isAuthenticated]);

    
    const saveCartToServer = useCallback(async (cartData: CartTotalDto): Promise<boolean> => {
        
        if (saveTimeoutRef.current) {
            clearTimeout(saveTimeoutRef.current);
        }

        return new Promise((resolve) => {
            saveTimeoutRef.current = setTimeout(async () => {
                if (!isMountedRef.current) {
                    resolve(false);
                    return;
                }

                setIsSaving(true);
                try {
                    const success = await saveCart(cartData);
                    if (!success && isMountedRef.current) {
                        setError('Failed to save cart');
                    }
                    resolve(success);
                } catch (err) {
                    console.error('Error saving cart:', err);
                    if (isMountedRef.current) {
                        setError('Failed to save cart');
                    }
                    resolve(false);
                } finally {
                    if (isMountedRef.current) {
                        setIsSaving(false);
                    }
                }
            }, SAVE_DEBOUNCE_MS);
        });
    }, []);

    
    const addToCart = useCallback(async (product: Product, quantity: number = 1): Promise<boolean> => {
        if (!isAuthenticated) {
            setError('Please sign in to add items to cart');
            return false;
        }

        setError(null);

        
        const currentCart = cart || {
            discountCode: "",
            cartItems: [],
            totalPrice: 0
        };

        const existingItemIndex = currentCart.cartItems.findIndex(
            item => item.productId === product.productId
        );

        let updatedItems: CartItemDto[];

        if (existingItemIndex >= 0) {
            
            updatedItems = currentCart.cartItems.map((item, index) =>
                index === existingItemIndex
                    ? { ...item, quantity: item.quantity + quantity }
                    : item
            );
        } else {
            
            const newItem: CartItemDto = {
                productId: product.productId,
                productName: product.productName,
                price: product.productPrice,
                productImageUrl: product.productImageUrl,
                quantity,
                categoryId: product.categoryId
            };
            updatedItems = [...currentCart.cartItems, newItem];
        }

        const updatedCart: CartTotalDto = {
            ...currentCart,
            cartItems: updatedItems,
            totalPrice: updatedItems.reduce((acc, item) => acc + (item.price * item.quantity), 0)
        };

        
        setCart(updatedCart);

        
        const success = await saveCartToServer(updatedCart);

        if (!success) {
            
            setCart(currentCart);
        }

        return success;
    }, [cart, isAuthenticated, saveCartToServer]);

    
    const removeFromCart = useCallback(async (productId: string): Promise<boolean> => {
        if (!cart) return false;

        setError(null);

        const updatedItems = cart.cartItems.filter(item => item.productId !== productId);

        const updatedCart: CartTotalDto = {
            ...cart,
            cartItems: updatedItems,
            totalPrice: updatedItems.reduce((acc, item) => acc + (item.price * item.quantity), 0)
        };

        
        const previousCart = cart;
        setCart(updatedCart);

        
        const success = await saveCartToServer(updatedCart);

        if (!success) {
            
            setCart(previousCart);
        }

        return success;
    }, [cart, saveCartToServer]);

    
    const updateQuantity = useCallback(async (productId: string, quantity: number): Promise<boolean> => {
        if (!cart) return false;
        if (quantity < 1) return removeFromCart(productId);

        setError(null);

        const updatedItems = cart.cartItems.map(item =>
            item.productId === productId
                ? { ...item, quantity }
                : item
        );

        const updatedCart: CartTotalDto = {
            ...cart,
            cartItems: updatedItems,
            totalPrice: updatedItems.reduce((acc, item) => acc + (item.price * item.quantity), 0)
        };

        
        const previousCart = cart;
        setCart(updatedCart);

        
        const success = await saveCartToServer(updatedCart);

        if (!success) {
            
            setCart(previousCart);
        }

        return success;
    }, [cart, removeFromCart, saveCartToServer]);

    
    const clearCart = useCallback(async (): Promise<boolean> => {
        if (!cart) return true;

        setError(null);
        setIsSaving(true);

        
        const previousCart = cart;
        setCart(null);

        try {
            const success = await deleteCart();

            if (!success && isMountedRef.current) {
                
                setCart(previousCart);
                setError('Failed to clear cart');
            }

            return success;
        } catch (err) {
            console.error('Error clearing cart:', err);
            if (isMountedRef.current) {
                setCart(previousCart);
                setError('Failed to clear cart');
            }
            return false;
        } finally {
            if (isMountedRef.current) {
                setIsSaving(false);
            }
        }
    }, [cart]);

    
    const clearError = useCallback(() => {
        setError(null);
    }, []);

    
    
    

    
    useEffect(() => {
        isMountedRef.current = true;

        if (!authLoading) {
            refreshCart();
        }

        return () => {
            isMountedRef.current = false;
            if (saveTimeoutRef.current) {
                clearTimeout(saveTimeoutRef.current);
            }
        };
    }, [isAuthenticated, authLoading, refreshCart]);

    
    
    

    const value: CartContextType = {
        cart,
        itemCount,
        isLoading,
        isSaving,
        error,
        addToCart,
        removeFromCart,
        updateQuantity,
        clearCart,
        refreshCart,
        clearError
    };

    return (
        <CartContext.Provider value={value}>
            {children}
        </CartContext.Provider>
    );
}






export function useCart(): CartContextType {
    const context = useContext(CartContext);

    if (context === undefined) {
        throw new Error('useCart must be used within a CartProvider');
    }

    return context;
}
