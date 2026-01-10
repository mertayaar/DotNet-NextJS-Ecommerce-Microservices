"use client";



import { useState, useEffect } from "react";
import Image from "next/image";
import Link from "next/link";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { getProducts } from "@/services/api";
import { Product } from "@/types/product";
import { AlertCircle, RefreshCw } from "lucide-react";
import { AddToCartButton } from "@/components/AddToCartButton";





interface ProductListProps {
    
    category?: string;
    
    className?: string;
}






function ProductSkeleton() {
    return (
        <Card className="overflow-hidden border-none shadow-none">
            <CardContent className="p-0">
                {}
                <div className="aspect-[3/4] rounded-lg bg-muted animate-pulse" />
                {}
                <div className="pt-4 space-y-2">
                    <div className="h-4 bg-muted rounded animate-pulse w-3/4" />
                    <div className="h-4 bg-muted rounded animate-pulse w-1/4" />
                </div>
            </CardContent>
        </Card>
    );
}





interface ErrorStateProps {
    message: string;
    onRetry: () => void;
}


function ErrorState({ message, onRetry }: ErrorStateProps) {
    return (
        <div className="flex flex-col items-center justify-center py-16 text-center">
            <AlertCircle className="h-12 w-12 text-destructive mb-4" />
            <h3 className="text-lg font-semibold mb-2">Unable to load products</h3>
            <p className="text-muted-foreground mb-6 max-w-md">{message}</p>
            <Button onClick={onRetry} variant="outline">
                <RefreshCw className="mr-2 h-4 w-4" />
                Try Again
            </Button>
        </div>
    );
}





interface ProductCardProps {
    product: Product;
}


function ProductCard({ product }: ProductCardProps) {
    return (
        <Card className="overflow-hidden border-none shadow-none group relative">
            <CardContent className="p-0">
                {}
                <div className="aspect-[3/4] relative overflow-hidden rounded-lg bg-muted">
                    <Link href={`/products/${product.productSlug || product.productId}`}>
                        <Image
                            src={product.productImageUrl.startsWith("http") ? product.productImageUrl : `${process.env.NEXT_PUBLIC_BACKEND_URL || "http://localhost:5248"}${product.productImageUrl}`}
                            alt={product.productName}
                            fill
                            className="object-cover transition-transform duration-300 group-hover:scale-105"
                            sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
                        />
                    </Link>
                    {}
                    <div className="absolute inset-x-0 bottom-0 p-4 translate-y-full transition-transform duration-300 group-hover:translate-y-0 hidden md:block z-10">
                        <AddToCartButton product={product} variant="overlay" />
                    </div>
                </div>

                {}
                <div className="pt-4 space-y-1">
                    <Link href={`/products/${product.productSlug || product.productId}`} className="hover:underline">
                        <h3 className="font-medium leading-none">{product.productName}</h3>
                    </Link>
                    <p className="text-sm text-muted-foreground">
                        ${product.productPrice.toFixed(2)}
                    </p>
                </div>

                {}
                <div className="mt-4 md:hidden">
                    <AddToCartButton product={product} size="sm" />
                </div>
            </CardContent>
        </Card>
    );
}





export default function ProductList({ category = "all", className = "" }: ProductListProps) {
    
    
    

    const [products, setProducts] = useState<Product[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    
    
    

    
    const loadProducts = async () => {
        setIsLoading(true);
        setError(null);

        try {
            
            const data = await getProducts();

            
            const filteredProducts = category === "all"
                ? data
                : data.filter((p) => p.categoryId === category);

            setProducts(filteredProducts);
        } catch (err) {
            const error = err as Error;
            setError(error.message || "Failed to fetch products");
            console.error("Error fetching products:", error);
        } finally {
            setIsLoading(false);
        }
    };

    
    
    

    
    useEffect(() => {
        loadProducts();
        
    }, [category]);

    
    
    

    
    if (isLoading) {
        return (
            <div className={`grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 ${className}`}>
                {[...Array(6)].map((_, i) => (
                    <ProductSkeleton key={i} />
                ))}
            </div>
        );
    }

    
    if (error) {
        return <ErrorState message={error} onRetry={loadProducts} />;
    }

    
    if (products.length === 0) {
        return (
            <div className="text-center py-16">
                <p className="text-muted-foreground">No products found in this category.</p>
            </div>
        );
    }

    
    return (
        <div className={`grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 ${className}`}>
            {products.map((product) => (
                <ProductCard key={product.productId} product={product} />
            ))}
        </div>
    );
}
