"use client"

import { Button } from "@/components/ui/button"
import Link from "next/link"
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { useAuth } from "@/contexts/AuthContext"
import { useRouter } from "next/navigation"
import { useEffect, useState } from "react"
import { Loader2, Package } from "lucide-react"
import { OrderingDto, OrderDetailDto, getOrderingByUserId, getOrderDetails } from "@/services/api"
import { format } from "date-fns"
import Image from "next/image"

interface OrderWithDetails extends OrderingDto {
    items: OrderDetailDto[];
}

export default function ProfilePage() {
    const { user, isAuthenticated, isLoading: authLoading, logout } = useAuth()
    const router = useRouter()
    const [orders, setOrders] = useState<OrderWithDetails[]>([])
    const [loadingOrders, setLoadingOrders] = useState(false)

    
    useEffect(() => {
        if (!authLoading && !isAuthenticated) {
            router.push('/login?returnUrl=/profile')
        }
    }, [authLoading, isAuthenticated, router])

    
    useEffect(() => {
        if (user && user.sub) {
            const fetchOrders = async () => {
                setLoadingOrders(true)
                try {
                    const userId = user.sub || "";
                    if (userId) {
                        const orderData = await getOrderingByUserId(userId)

                        
                        const ordersWithDetails = await Promise.all(
                            orderData.map(async (order) => {
                                const items = await getOrderDetails(order.orderingId.toString())
                                return { ...order, items }
                            })
                        )

                        setOrders(ordersWithDetails)
                    }
                } catch (error) {
                    console.error("Failed to fetch orders", error)
                } finally {
                    setLoadingOrders(false)
                }
            }
            fetchOrders()
        }
    }, [user])

    
    const handleLogout = async () => {
        await logout()
        router.push('/')
    }

    
    const getInitials = (name?: string) => {
        if (!name) return 'U'
        return name
            .split(' ')
            .map(n => n[0])
            .join('')
            .toUpperCase()
            .slice(0, 2)
    }

    
    const getImageUrl = (imageUrl: string) => {
        if (!imageUrl) return '/placeholder.png'
        if (imageUrl.startsWith('http')) return imageUrl
        return `${process.env.NEXT_PUBLIC_BACKEND_URL || ''}${imageUrl}`
    }

    
    if (authLoading) {
        return (
            <div className="container py-20 flex justify-center items-center">
                <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
        )
    }

    
    if (!isAuthenticated || !user) {
        return null
    }

    return (
        <div className="container py-10">
            <h1 className="text-3xl font-bold tracking-tight mb-8">My Account</h1>

            <div className="grid gap-8 md:grid-cols-[1fr_200px] lg:grid-cols-[1fr_300px]">
                <div className="space-y-6">
                    <Card>
                        <CardHeader>
                            <CardTitle>Profile Details</CardTitle>
                            <CardDescription>Your personal information.</CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <div className="flex items-center gap-4">
                                <Avatar className="h-16 w-16">
                                    <AvatarImage src={user.picture} />
                                    <AvatarFallback>{getInitials(user.name)}</AvatarFallback>
                                </Avatar>
                                <div>
                                    <h3 className="font-semibold text-lg">
                                        {user.name || user.givenName || 'User'}
                                    </h3>
                                    <p className="text-sm text-muted-foreground">
                                        {user.email || 'No email'}
                                    </p>
                                    {user.roles && user.roles.length > 0 && (
                                        <p className="text-xs text-muted-foreground mt-1">
                                            Role: {user.roles.join(', ')}
                                        </p>
                                    )}
                                </div>
                            </div>
                            <div className="flex gap-4">
                                <Button variant="outline" asChild>
                                    <Link href="/profile/edit">Edit Profile</Link>
                                </Button>
                                <Button variant="outline" asChild>
                                    <Link href="/profile/addresses">Manage Addresses</Link>
                                </Button>
                                <Button variant="destructive" onClick={handleLogout}>
                                    Sign Out
                                </Button>
                            </div>
                        </CardContent>
                    </Card>

                    <Card>
                        <CardHeader>
                            <CardTitle>Order History</CardTitle>
                            <CardDescription>View your past orders.</CardDescription>
                        </CardHeader>
                        <CardContent>
                            {loadingOrders ? (
                                <div className="flex justify-center py-4">
                                    <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
                                </div>
                            ) : orders.length === 0 ? (
                                <div className="text-center py-8 text-muted-foreground">
                                    <Package className="h-12 w-12 mx-auto mb-2 opacity-50" />
                                    <p>No orders found.</p>
                                </div>
                            ) : (
                                <div className="space-y-6">
                                    {orders.map((order) => (
                                        <div key={order.orderingId} className="border-b pb-6 last:border-0 last:pb-0 space-y-4">
                                            <div className="flex justify-between items-start">
                                                <div>
                                                    <p className="font-medium">Order #{order.orderingId}</p>
                                                    <p className="text-sm text-muted-foreground">
                                                        Placed on {format(new Date(order.orderDate), "MMM dd, yyyy")}
                                                    </p>
                                                </div>
                                                <div className="text-right">
                                                    <p className="font-medium">${order.totalPrice.toFixed(2)}</p>
                                                    <p className="text-sm text-green-600">Completed</p>
                                                </div>
                                            </div>

                                            <div className="flex justify-between items-end">
                                                {}
                                                <div className="flex gap-2">
                                                    {order.items && order.items.length > 0 && (
                                                        <>
                                                            {order.items.slice(0, 3).map((item) => (
                                                                <div
                                                                    key={item.orderDetailId}
                                                                    className="relative w-12 h-12 rounded-md overflow-hidden border bg-gray-100"
                                                                >
                                                                    <Image
                                                                        src={getImageUrl(item.productImageUrl)}
                                                                        alt={item.productName}
                                                                        fill
                                                                        className="object-cover"
                                                                        sizes="48px"
                                                                    />
                                                                    {item.productAmount > 1 && (
                                                                        <span className="absolute -top-1 -right-1 bg-primary text-primary-foreground text-xs rounded-full w-4 h-4 flex items-center justify-center">
                                                                            {item.productAmount}
                                                                        </span>
                                                                    )}
                                                                </div>
                                                            ))}
                                                            {order.items.length > 3 && (
                                                                <div className="w-12 h-12 rounded-md border bg-gray-100 flex items-center justify-center text-sm font-medium text-muted-foreground">
                                                                    +{order.items.length - 3}
                                                                </div>
                                                            )}
                                                        </>
                                                    )}
                                                </div>

                                                <Button variant="outline" size="sm" asChild>
                                                    <Link href={`/profile/orders/${order.orderingId}`}>
                                                        View Details
                                                    </Link>
                                                </Button>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    )
}
