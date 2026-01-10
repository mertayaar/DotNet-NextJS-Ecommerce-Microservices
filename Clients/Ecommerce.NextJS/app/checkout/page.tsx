"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import Link from "next/link"
import { CheckoutForm, CheckoutFormValues } from "@/components/CheckoutForm"
import {
    createCargoCustomer,
    createOrder,
    CreateCargoCustomerDto,
    CreateOrderDto
} from "@/services/api"
import { getMyCart, deleteCart } from "@/services/cartService"
import { getAddresses, Address } from "@/services/addressService"
import { CartTotalDto } from "@/types/cart"
import { useAuth } from "@/contexts/AuthContext"
import { useCart } from "@/contexts/CartContext"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { Loader2, ArrowLeft } from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { toast } from "sonner"

export default function CheckoutPage() {
    const router = useRouter()
    const { user, isAuthenticated } = useAuth()
    const { refreshCart } = useCart()
    const [cart, setCart] = useState<CartTotalDto | null>(null)
    const [savedAddresses, setSavedAddresses] = useState<Address[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const [isSubmitting, setIsSubmitting] = useState(false)

    useEffect(() => {
        loadBasket()
        if (isAuthenticated) {
            loadAddresses()
        }
    }, [isAuthenticated])

    const loadBasket = async () => {
        try {
            const data = await getMyCart()
            if (!data || !data.cartItems || data.cartItems.length === 0) {
                setCart(null)
            } else {
                setCart(data)
            }
        } catch (error) {
            console.error("Failed to load basket", error)
        } finally {
            setIsLoading(false)
        }
    }

    const loadAddresses = async () => {
        try {
            const addresses = await getAddresses()
            setSavedAddresses(addresses)
        } catch (error) {
            console.error("Failed to load addresses", error)
        }
    }

    
    const cartItems = cart?.cartItems || []
    const subtotal = cartItems.reduce((acc, item) => acc + item.price * item.quantity, 0)
    const discountRate = cart?.discountRate || 0
    const discountAmount = subtotal * (discountRate / 100)
    const finalTotal = subtotal - discountAmount

    const handleCheckout = async (values: CheckoutFormValues) => {
        if (!cart) return

        
        const userId = cart.userId || user?.sub;

        if (!userId) {
            toast.error("User ID missing. Please log in again.");
            return;
        }

        setIsSubmitting(true)
        try {
            
            const cargoDto: CreateCargoCustomerDto = {
                name: values.name,
                surname: values.surname,
                email: values.email,
                phone: values.phone,
                city: values.city,
                district: values.district,
                address: values.address,
                userCustomerId: userId
            }

            const cargoSuccess = await createCargoCustomer(cargoDto)
            if (!cargoSuccess) {
                throw new Error("Failed to save shipping information")
            }

            
            const orderDto: CreateOrderDto = {
                userId: userId,
                totalPrice: finalTotal, 
                orderDate: new Date().toISOString(),
                discountCode: cart.discountCode || undefined,
                discountRate: cart.discountRate || undefined,
                orderItems: cartItems.map(item => ({
                    productId: item.productId,
                    productName: item.productName,
                    productPrice: item.price,
                    productAmount: item.quantity,
                    productTotalPrice: item.price * item.quantity,
                    productImageUrl: item.productImageUrl
                }))
            }

            const orderSuccess = await createOrder(orderDto)
            if (!orderSuccess) {
                throw new Error("Failed to create order")
            }

            
            await deleteCart()
            await refreshCart()

            toast.success("Order Placed Successfully!", {
                description: "Thank you for your purchase. Redirecting...",
            })

            
            router.push("/profile")

        } catch (error) {
            console.error("Checkout failed:", error)
            toast.error("Checkout Failed", {
                description: "There was a problem processing your order. Please try again."
            })
            
            setIsSubmitting(false)
        }
    }

    if (isLoading) {
        return (
            <div className="flex h-screen items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
        )
    }

    if (!cart) {
        return (
            <div className="container py-20 text-center">
                <h1 className="text-2xl font-bold mb-4">Your Cart is Empty</h1>
                <p className="text-muted-foreground mb-8">Add some items to your cart before checking out.</p>
                <button
                    onClick={() => router.push("/products")}
                    className="text-primary hover:underline"
                >
                    Browse Products
                </button>
            </div>
        )
    }

    return (
        <div className="container py-10">
            <div className="flex items-center gap-4 mb-8">
                <Button variant="ghost" asChild className="pl-0 hover:pl-0 hover:bg-transparent text-muted-foreground hover:text-foreground">
                    <Link href="/cart" className="flex items-center gap-2">
                        <ArrowLeft className="h-4 w-4" />
                        Back to Cart
                    </Link>
                </Button>
                <h1 className="text-3xl font-bold">Checkout</h1>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {}
                <div className="lg:col-span-2">
                    <Card>
                        <CardHeader>
                            <CardTitle>Shipping Information</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <CheckoutForm onSubmit={handleCheckout} isLoading={isSubmitting} savedAddresses={savedAddresses} />
                        </CardContent>
                    </Card>
                </div>

                {}
                <div>
                    <Card>
                        <CardHeader>
                            <CardTitle>Order Summary</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="space-y-2">
                                {cart.cartItems.map((item) => (
                                    <div key={item.productId} className="flex justify-between text-sm">
                                        <span>{item.productName} x {item.quantity}</span>
                                        <span>${(item.price * item.quantity).toFixed(2)}</span>
                                    </div>
                                ))}
                            </div>

                            <Separator />

                            <div className="space-y-1.5">
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">Subtotal</span>
                                    <span>${subtotal.toFixed(2)}</span>
                                </div>
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">Shipping</span>
                                    <span>Free</span>
                                </div>
                                {discountAmount > 0 && (
                                    <div className="flex justify-between text-sm text-green-600">
                                        <span>Discount ({discountRate}%)</span>
                                        <span>-${discountAmount.toFixed(2)}</span>
                                    </div>
                                )}
                                <div className="flex justify-between font-bold text-lg pt-2">
                                    <span>Total</span>
                                    <span>${Math.max(0, finalTotal).toFixed(2)}</span>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    )
}
