"use client"

import { useEffect, useState, use } from "react"
import { useRouter } from "next/navigation"
import Image from "next/image"
import Link from "next/link"
import { format } from "date-fns"
import { Archive, ArrowLeft, Loader2, Package } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { useAuth } from "@/contexts/AuthContext"
import {
    getOrderingById,
    getOrderDetails,
    OrderingDto,
    OrderDetailDto,
} from "@/services/api"
import { toast } from "sonner"

interface OrderDetailWithProduct extends OrderDetailDto {
    productImage?: string;
    productSlug?: string;
}

export default function OrderDetailPage({ params }: { params: Promise<{ id: string }> }) {
    const router = useRouter()
    const { user, isLoading: authLoading } = useAuth()

    
    const { id } = use(params)

    const [order, setOrder] = useState<OrderingDto | null>(null)
    const [details, setDetails] = useState<OrderDetailWithProduct[]>([])
    const [isLoading, setIsLoading] = useState(true)

    useEffect(() => {
        if (!authLoading && !user) {
            router.push(`/login?returnUrl=/profile/orders/${id}`)
            return
        }

        if (user) {
            fetchOrderData()
        }
    }, [user, authLoading, id])

    const fetchOrderData = async () => {
        try {
            setIsLoading(true)

            
            const orderData = await getOrderingById(id)
            if (!orderData) {
                toast.error("Order not found")
                router.push("/profile")
                return
            }
            setOrder(orderData)

            
            const detailsData = await getOrderDetails(id)

            
            const enrichedDetails = detailsData.map(item => ({
                ...item,
                productImage: item.productImageUrl,
                productSlug: item.productName.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)/g, '')
            }))

            setDetails(enrichedDetails)

        } catch (error) {
            console.error("Failed to load order:", error)
            toast.error("Failed to load order details")
        } finally {
            setIsLoading(false)
        }
    }

    if (authLoading || isLoading) {
        return (
            <div className="container py-20 flex justify-center items-center h-[60vh]">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
        )
    }

    if (!order) {
        return null
    }

    return (
        <div className="container py-10 max-w-4xl">
            <div className="mb-6">
                <Button variant="ghost" asChild className="pl-0 hover:pl-0 hover:bg-transparent text-muted-foreground hover:text-foreground">
                    <Link href="/profile" className="flex items-center gap-2">
                        <ArrowLeft className="h-4 w-4" />
                        Back to Orders
                    </Link>
                </Button>
            </div>

            <div className="flex flex-col md:flex-row justify-between items-start gap-4 mb-8">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight flex items-center gap-3">
                        Order #{order.orderingId}
                        <span className="text-sm font-normal py-1 px-3 rounded-full bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">
                            Pending
                        </span>
                    </h1>
                    <p className="text-muted-foreground mt-2">
                        Placed on {format(new Date(order.orderDate), "MMMM dd, yyyy 'at' h:mm a")}
                    </p>
                </div>
            </div>

            <div className="grid gap-8">
                <Card>
                    <CardHeader>
                        <CardTitle>Order Items</CardTitle>
                        <CardDescription>{details.length} item(s) in this order</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-6">
                            {details.map((item) => (
                                <div key={item.orderDetailId} className="flex flex-col sm:flex-row gap-4 py-4 border-b last:border-0 last:pb-0">
                                    {}
                                    <Link href={`/products/${item.productSlug}`} className="relative h-24 w-24 flex-shrink-0 overflow-hidden rounded-md border bg-muted hover:opacity-80 transition-opacity">
                                        {item.productImage ? (
                                            <Image
                                                src={item.productImage}
                                                alt={item.productName}
                                                fill
                                                className="object-cover"
                                            />
                                        ) : (
                                            <div className="flex h-full w-full items-center justify-center bg-gray-100 dark:bg-gray-800">
                                                <Package className="h-8 w-8 text-muted-foreground opacity-50" />
                                            </div>
                                        )}
                                    </Link>

                                    {}
                                    <div className="flex flex-1 flex-col justify-between">
                                        <div className="flex justify-between items-start gap-2">
                                            <div>
                                                <Link
                                                    href={`/products/${item.productSlug}`}
                                                    className="font-semibold text-lg hover:text-primary hover:underline transition-colors"
                                                >
                                                    {item.productName}
                                                </Link>
                                            </div>
                                            <p className="font-semibold text-lg">
                                                ${item.productTotalPrice.toFixed(2)}
                                            </p>
                                        </div>

                                        <div className="flex justify-between items-end mt-4 sm:mt-0">
                                            <p className="text-sm text-muted-foreground">
                                                {item.productAmount} x ${item.productPrice.toFixed(2)}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader>
                        <CardTitle>Order Summary</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                        <div className="flex justify-between">
                            <span className="text-muted-foreground">Subtotal</span>
                            <span>
                                ${order.discountRate && order.discountRate > 0
                                    ? (order.totalPrice / (1 - order.discountRate / 100)).toFixed(2)
                                    : order.totalPrice.toFixed(2)
                                }
                            </span>
                        </div>
                        {order.discountCode && order.discountRate && order.discountRate > 0 && (
                            <div className="flex justify-between text-green-600">
                                <span className="flex items-center gap-2">
                                    <span className="bg-green-100 text-green-700 px-2 py-0.5 rounded text-xs font-medium">
                                        {order.discountCode}
                                    </span>
                                    <span>Discount ({order.discountRate}%)</span>
                                </span>
                                <span>-${((order.totalPrice / (1 - order.discountRate / 100)) - order.totalPrice).toFixed(2)}</span>
                            </div>
                        )}
                        <div className="flex justify-between">
                            <span className="text-muted-foreground">Shipping</span>
                            <span>Free</span>
                        </div>
                        <Separator className="my-2" />
                        <div className="flex justify-between font-bold text-lg">
                            <span>Total</span>
                            <span>${order.totalPrice.toFixed(2)}</span>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}
