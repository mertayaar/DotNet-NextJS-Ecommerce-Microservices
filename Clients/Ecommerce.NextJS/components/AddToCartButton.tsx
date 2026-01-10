"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { useCart } from "@/contexts/CartContext"
import { useAuth } from "@/contexts/AuthContext"
import { Product } from "@/types/product"
import { Loader2, ShoppingBag, Check } from "lucide-react"
import { useRouter } from "next/navigation"
import { cn } from "@/lib/utils"

interface AddToCartButtonProps {
    product: Product
    variant?: "default" | "overlay" | "icon"
    className?: string
    showText?: boolean
    size?: "default" | "sm" | "lg" | "icon"
}

export function AddToCartButton({
    product,
    variant = "default",
    className,
    showText = true,
    size = "default"
}: AddToCartButtonProps) {
    const { addToCart } = useCart()
    const { isAuthenticated } = useAuth()
    const router = useRouter()

    
    const [status, setStatus] = useState<"idle" | "loading" | "success">("idle")

    const handleAddToCart = async (e: React.MouseEvent) => {
        e.preventDefault() 
        e.stopPropagation()

        if (!isAuthenticated) {
            router.push('/login?returnUrl=' + window.location.pathname)
            return
        }

        try {
            setStatus("loading")
            await addToCart(product)
            setStatus("success")

            
            setTimeout(() => {
                setStatus("idle")
            }, 2000)
        } catch (error) {
            console.error("Failed to add to cart", error)
            setStatus("idle")
        }
    }

    
    if (variant === "overlay") {
        return (
            <Button
                className={cn(
                    "w-full bg-white/90 text-black hover:bg-white shadow-lg backdrop-blur-sm transition-all duration-300",
                    status === "success" && "bg-green-600/90 text-white hover:bg-green-600",
                    className
                )}
                onClick={handleAddToCart}
                disabled={status === "loading"}
                size={size}
            >
                {status === "loading" ? (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : status === "success" ? (
                    <Check className="mr-2 h-4 w-4" />
                ) : (
                    <ShoppingBag className="mr-2 h-4 w-4" />
                )}
                {status === "success" ? "Added to Cart" : "Add to Cart"}
            </Button>
        )
    }

    
    return (
        <Button
            className={cn(
                "transition-all duration-300",
                status === "success" && "bg-green-600 hover:bg-green-700 text-white",
                className
            )}
            onClick={handleAddToCart}
            disabled={status === "loading"}
            size={size}
        >
            {status === "loading" ? (
                <Loader2 className={cn("h-4 w-4 animate-spin", showText && "mr-2")} />
            ) : status === "success" ? (
                <Check className={cn("h-4 w-4", showText && "mr-2")} />
            ) : (
                <ShoppingBag className={cn("h-4 w-4", showText && "mr-2")} />
            )}

            {showText && (
                status === "success" ? "Added to Cart" :
                    status === "loading" ? "Adding..." : "Add to Cart"
            )}
        </Button>
    )
}
