"use client";




import { useState, useEffect } from "react";
import { useParams } from "next/navigation";
import Image from "next/image";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { getProductBySlug, getCategory, CategoryDto } from "@/services/api";
import { Product } from "@/types/product";
import { ArrowLeft, AlertCircle, RefreshCw } from "lucide-react";
import { AddToCartButton } from "@/components/AddToCartButton";






function ProductDetailSkeleton() {
    return (
        <div className="container py-10 md:py-20">
            {}
            <div className="h-5 w-32 bg-muted rounded animate-pulse mb-8" />

            <div className="grid grid-cols-1 md:grid-cols-2 gap-10 lg:gap-16">
                {}
                <div className="relative aspect-square overflow-hidden rounded-lg bg-muted animate-pulse" />

                {}
                <div className="flex flex-col justify-center space-y-6">
                    <div className="space-y-4">
                        <div className="h-10 bg-muted rounded animate-pulse w-3/4" />
                        <div className="h-6 bg-muted rounded animate-pulse w-1/4" />
                    </div>
                    <div className="space-y-2">
                        <div className="h-4 bg-muted rounded animate-pulse w-full" />
                        <div className="h-4 bg-muted rounded animate-pulse w-5/6" />
                        <div className="h-4 bg-muted rounded animate-pulse w-4/6" />
                    </div>
                    <div className="h-12 bg-muted rounded animate-pulse w-48 mt-6" />
                </div>
            </div>
        </div>
    );
}





interface ErrorStateProps {
    message: string;
    onRetry: () => void;
    isNotFound?: boolean;
}


function ErrorState({ message, onRetry, isNotFound }: ErrorStateProps) {
    return (
        <div className="container py-20">
            <div className="flex flex-col items-center justify-center text-center">
                <AlertCircle className="h-16 w-16 text-destructive mb-6" />
                <h2 className="text-2xl font-bold mb-2">
                    {isNotFound ? "Product Not Found" : "Unable to Load Product"}
                </h2>
                <p className="text-muted-foreground mb-8 max-w-md">{message}</p>
                <div className="flex gap-4">
                    <Button variant="outline" asChild>
                        <Link href="/products">
                            <ArrowLeft className="mr-2 h-4 w-4" />
                            Back to Products
                        </Link>
                    </Button>
                    {!isNotFound && (
                        <Button onClick={onRetry}>
                            <RefreshCw className="mr-2 h-4 w-4" />
                            Try Again
                        </Button>
                    )}
                </div>
            </div>
        </div>
    );
}





export default function ProductDetailPage() {
    
    
    

    const params = useParams();
    const productSlug = params.slug as string;

    
    
    

    const [product, setProduct] = useState<Product | null>(null);
    const [category, setCategory] = useState<CategoryDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [isNotFound, setIsNotFound] = useState(false);

    
    
    

    
    const loadProduct = async () => {
        if (!productSlug) {
            setError("Invalid product URL");
            setIsLoading(false);
            return;
        }

        setIsLoading(true);
        setError(null);
        setIsNotFound(false);

        try {
            
            const data = await getProductBySlug(productSlug);

            if (!data) {
                setIsNotFound(true);
                setError("The product you're looking for doesn't exist or has been removed.");
                setProduct(null);
            } else {
                setProduct(data);
                
                if (data.categoryId) {
                    const categoryData = await getCategory(data.categoryId);
                    setCategory(categoryData);
                }
            }
        } catch (err) {
            console.error("Failed to fetch product:", err);
            setError("An unexpected error occurred. Please try again.");
        } finally {
            setIsLoading(false);
        }
    };

    
    
    

    
    useEffect(() => {
        loadProduct();
        
    }, [productSlug]);

    
    
    

    
    if (isLoading) {
        return <ProductDetailSkeleton />;
    }

    
    if (error || !product) {
        return (
            <ErrorState
                message={error || "Product not found"}
                onRetry={loadProduct}
                isNotFound={isNotFound}
            />
        );
    }

    
    return (
        <div className="container py-10 md:py-20">
            {}
            <Link
                href="/products"
                className="inline-flex items-center text-sm text-muted-foreground hover:text-foreground mb-8"
            >
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to Products
            </Link>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-10 lg:gap-16">
                {}
                <div className="relative aspect-square overflow-hidden rounded-lg bg-muted">
                    <Image
                        src={product.productImageUrl.startsWith("http") ? product.productImageUrl : `${process.env.NEXT_PUBLIC_BACKEND_URL || "http://localhost:5248"}${product.productImageUrl}`}
                        alt={product.productName}
                        fill
                        className="object-cover"
                        priority
                        sizes="(max-width: 768px) 100vw, 50vw"
                    />
                </div>

                {}
                <div className="flex flex-col justify-center space-y-6">
                    <div className="space-y-2">
                        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
                            {product.productName}
                        </h1>
                        <p className="text-xl text-muted-foreground">
                            ${product.productPrice.toFixed(2)}
                        </p>
                    </div>

                    <p className="text-base text-muted-foreground leading-relaxed">
                        {product.productDescription}
                    </p>

                    {}
                    <div className="pt-6">
                        <AddToCartButton
                            product={product}
                            size="lg"
                            className="w-full md:w-auto text-base px-8"
                        />
                    </div>

                    {}
                    <div className="pt-8 border-t">
                        <div className="grid grid-cols-2 gap-4 text-sm">
                            <div>
                                <span className="font-semibold block mb-1">Category</span>
                                <span className="text-muted-foreground capitalize">
                                    {category?.categoryName || "Uncategorized"}
                                </span>
                            </div>
                            <div>
                                <span className="font-semibold block mb-1">Shipping</span>
                                <span className="text-muted-foreground">
                                    Free Standard Shipping
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
