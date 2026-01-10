"use client"

import Link from "next/link"
import Image from "next/image"
import { useState, useEffect } from "react"
import { Trash, Minus, Plus, ShoppingBag, ArrowRight } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { Input } from "@/components/ui/input"
import {
    Card,
    CardContent,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { CartItemDto, CartTotalDto } from "@/types/cart"
import { getMyCart, saveCart, deleteCart, applyCoupon, removeCoupon } from "@/services/cartService"
import { useRouter } from "next/navigation"
import { fetchCategories, CategoryDto } from "@/services/api"
import { useCart } from "@/contexts/CartContext"
import { useAuth } from "@/contexts/AuthContext"

export default function CartPage() {
    const router = useRouter()
    const { refreshCart } = useCart()
    const { isAuthenticated, isLoading: authLoading } = useAuth()
    const [cart, setCart] = useState<CartTotalDto | null>(null)
    const [loading, setLoading] = useState(true)
    const [couponCode, setCouponCode] = useState("")
    const [couponMessage, setCouponMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null)
    const [updating, setUpdating] = useState(false);
    const [categories, setCategories] = useState<Record<string, string>>({});

    useEffect(() => {
        
        if (authLoading) return;

        
        if (!isAuthenticated) {
            router.push("/login?returnUrl=/cart")
            return
        }

        const loadInitData = async () => {
            try {
                
                try {
                    const cats = await fetchCategories();
                    const catMap: Record<string, string> = {};
                    cats.forEach(c => catMap[c.categoryId] = c.categoryName);
                    setCategories(catMap);
                } catch (catError) {
                    console.error("Failed to load categories", catError);
                }

                
                const data = await getMyCart()
                setCart(data)
                if (data?.discountCode) {
                    setCouponCode(data.discountCode)
                    setCouponMessage({ type: 'success', text: `Coupon ${data.discountCode} applied` })
                }
            } catch (error) {
                console.error("Failed to load cart", error)
            } finally {
                setLoading(false)
            }
        }
        loadInitData()
    }, [router, isAuthenticated, authLoading])

    const updateCartInBackend = async (newCart: CartTotalDto) => {
        setUpdating(true)
        setCart(newCart)
        await saveCart(newCart)
        await refreshCart() 
        setUpdating(false)
    }

    const updateQuantity = async (productId: string, change: number) => {
        if (!cart) return

        const updatedItems = cart.cartItems.map(item => {
            if (item.productId === productId) {
                return { ...item, quantity: Math.max(1, item.quantity + change) }
            }
            return item
        })

        const updatedCart = { ...cart, cartItems: updatedItems }
        
        updatedCart.totalPrice = updatedItems.reduce((acc, item) => acc + (item.price * item.quantity), 0);

        await updateCartInBackend(updatedCart)
    }

    const removeItem = async (productId: string) => {
        if (!cart) return

        const updatedItems = cart.cartItems.filter(item => item.productId !== productId)

        if (updatedItems.length === 0) {
            setUpdating(true)
            await deleteCart()
            setCart(null)
            await refreshCart() 
            setUpdating(false)
        } else {
            const updatedCart = { ...cart, cartItems: updatedItems }
            updatedCart.totalPrice = updatedItems.reduce((acc, item) => acc + (item.price * item.quantity), 0);
            await updateCartInBackend(updatedCart)
        }
    }

    const handleApplyCoupon = async () => {
        if (!cart || !couponCode) return

        setUpdating(true)
        setCouponMessage(null);

        
        const result = await applyCoupon(couponCode);

        if (result.success) {
            
            const updatedCart = await getMyCart();
            setCart(updatedCart);
            await refreshCart();
            setCouponMessage({ type: 'success', text: result.message })
        } else {
            setCouponMessage({ type: 'error', text: result.message })
        }
        setUpdating(false)
    }

    const handleRemoveCoupon = async () => {
        if (!cart) return
        setUpdating(true)

        await removeCoupon();

        
        const updatedCart = await getMyCart();
        setCart(updatedCart);

        setCouponCode("");
        setCouponMessage(null);
        await refreshCart();
        setUpdating(false)
    }

    if (loading || authLoading) {
        return (
            <div className="container py-20 flex justify-center">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
            </div>
        )
    }

    const cartItems = cart?.cartItems || []
    const isEmpty = cartItems.length === 0

    if (isEmpty) {
        return (
            <div className="container flex flex-col items-center justify-center min-h-[60vh] py-10 text-center animate-in fade-in zoom-in duration-500">
                <div className="bg-muted/30 p-8 rounded-full mb-6 relative">
                    <ShoppingBag className="h-24 w-24 text-muted-foreground/50" />
                    <div className="absolute bottom-0 right-0 bg-background p-2 rounded-full shadow-sm border">
                        <Minus className="h-6 w-6 text-muted-foreground" />
                    </div>
                </div>
                <h1 className="text-4xl font-extrabold tracking-tight lg:text-5xl mb-4 bg-gradient-to-r from-primary to-primary/60 bg-clip-text text-transparent">
                    Your Cart is Empty
                </h1>
                <p className="text-xl text-muted-foreground mb-8 max-w-[600px]">
                    Looks like you haven't added anything to your cart yet.
                    Explore our premium collection and find something you love.
                </p>
                <Button asChild size="lg" className="rounded-full px-8 text-lg h-12 shadow-lg hover:shadow-xl transition-all hover:scale-105">
                    <Link href="/products" className="flex items-center gap-2">
                        Start Shopping <ArrowRight className="h-5 w-5" />
                    </Link>
                </Button>
            </div>
        )
    }

    const subtotal = cartItems.reduce((acc, item) => acc + item.price * item.quantity, 0)
    const shipping: number = 0 // Free
    const discountRate = cart?.discountRate || 0;
    const discountAmount = subtotal * (discountRate / 100);
    const total = subtotal + shipping - discountAmount;

    return (
        <div className="container py-10 animate-in fade-in slide-in-from-bottom-4 duration-500">
            <h1 className="text-3xl font-bold tracking-tight mb-8">Your Cart</h1>

            <div className="grid gap-8 lg:grid-cols-3">
                {/* Cart Items */}
                <div className="lg:col-span-2 space-y-4">
                    {cartItems.map((item) => (
                        <div key={item.productId} className="flex gap-4 p-4 border rounded-xl items-start bg-card text-card-foreground shadow-sm hover:shadow-md transition-shadow">
                            <div className="relative aspect-square h-24 w-24 min-w-[6rem] overflow-hidden rounded-lg bg-muted border">
                                <Image
                                    src={item.productImageUrl ? (item.productImageUrl.startsWith("http") ? item.productImageUrl : `${process.env.NEXT_PUBLIC_BACKEND_URL || "http://localhost:5248"}${item.productImageUrl}`) : "/placeholder.jpg"}
                                    alt={item.productName}
                                    fill
                                    className="object-cover"
                                />
                            </div>
                            <div className="flex flex-1 flex-col justify-between self-stretch">
                                <div className="flex justify-between gap-2">
                                    <div>
                                        <h3 className="font-semibold text-lg">{item.productName}</h3>
                                        <p className="text-sm text-muted-foreground">
                                            {item.categoryId && categories[item.categoryId]
                                                ? categories[item.categoryId]
                                                : "Product"}
                                        </p>
                                    </div>
                                    <p className="font-bold text-lg">${(item.price * item.quantity).toFixed(2)}</p>
                                </div>
                                <div className="flex justify-between items-end mt-4">
                                    <div className="flex items-center gap-2 border rounded-md bg-background">
                                        <Button
                                            variant="ghost"
                                            size="icon"
                                            className="h-8 w-8 rounded-r-none hover:bg-muted"
                                            onClick={() => updateQuantity(item.productId, -1)}
                                            disabled={updating}
                                        >
                                            <Minus className="h-3 w-3" />
                                        </Button>
                                        <span className="text-sm w-8 text-center font-medium">{item.quantity}</span>
                                        <Button
                                            variant="ghost"
                                            size="icon"
                                            className="h-8 w-8 rounded-l-none hover:bg-muted"
                                            onClick={() => updateQuantity(item.productId, 1)}
                                            disabled={updating}
                                        >
                                            <Plus className="h-3 w-3" />
                                        </Button>
                                    </div>
                                    <Button
                                        variant="ghost"
                                        size="sm"
                                        className="text-muted-foreground hover:text-destructive hover:bg-destructive/10"
                                        onClick={() => removeItem(item.productId)}
                                        disabled={updating}
                                    >
                                        <Trash className="h-4 w-4 mr-2" />
                                        Remove
                                    </Button>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                {/* Order Summary */}
                <div className="lg:col-span-1">
                    <Card className="shadow-lg border-primary/10 sticky top-24">
                        <CardHeader className="bg-muted/30 pb-4">
                            <CardTitle>Order Summary</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4 pt-6">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">Subtotal</span>
                                <span className="font-medium">${subtotal.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">Shipping</span>
                                <span className="text-green-600 font-medium">{shipping === 0 ? "Free" : `$${shipping.toFixed(2)}`}</span>
                            </div>
                            {discountAmount > 0 && (
                                <div className="flex justify-between text-sm text-green-600">
                                    <span>Discount ({discountRate}%)</span>
                                    <span>-${discountAmount.toFixed(2)}</span>
                                </div>
                            )}
                            <Separator />
                            <div className="flex justify-between font-bold text-lg">
                                <span className="text-primary">Total</span>
                                <span className="text-primary">${Math.max(0, total).toFixed(2)}</span>
                            </div>

                            <div className="pt-4 space-y-2">
                                <div className="flex gap-2">
                                    <Input
                                        placeholder="Coupon code"
                                        value={couponCode}
                                        onChange={(e) => setCouponCode(e.target.value)}
                                        className="bg-background"
                                        disabled={updating || !!(cart?.discountRate && cart.discountRate > 0)}
                                    />
                                    {cart?.discountRate && cart.discountRate > 0 ? (
                                        <Button variant="destructive" size="icon" onClick={handleRemoveCoupon} disabled={updating}>
                                            <Trash className="h-4 w-4" />
                                        </Button>
                                    ) : (
                                        <Button variant="outline" onClick={handleApplyCoupon} disabled={updating || !couponCode}>Apply</Button>
                                    )}
                                </div>
                                {couponMessage && (
                                    <p className={`text-xs ${couponMessage.type === 'success' ? 'text-green-600' : 'text-red-500'}`}>
                                        {couponMessage.text}
                                    </p>
                                )}
                            </div>
                        </CardContent>
                        <CardFooter className="pb-6">
                            <Button
                                className="w-full text-lg h-12 shadow-md"
                                size="lg"
                                disabled={isEmpty || updating}
                                onClick={() => router.push("/checkout")}
                            >
                                Proceed to Checkout
                            </Button>
                        </CardFooter>
                    </Card>
                </div>
            </div>
        </div>
    )
}
