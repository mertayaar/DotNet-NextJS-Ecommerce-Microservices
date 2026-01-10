"use client";



import { useState, useEffect } from "react";
import Marquee from "@/components/marquee"
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import Image from "next/image"
import Link from "next/link"
import { getProducts } from "@/services/api";
import { Product } from "@/types/product"
import { Loader2 } from "lucide-react"
import { AddToCartButton } from "@/components/AddToCartButton"





interface FeaturedCarouselProps {
  products: Product[];
  isLoading: boolean;
}

function FeaturedCarousel({ products, isLoading }: FeaturedCarouselProps) {
  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-20">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (products.length === 0) {
    return (
      <div className="text-center py-10 text-muted-foreground">
        No products available at the moment.
      </div>
    );
  }

  return (
    <Carousel
      opts={{
        align: "start",
        loop: true,
      }}
      className="w-full"
    >
      <CarouselContent className="-ml-4">
        {products.map((product) => (
          <CarouselItem key={product.productId} className="pl-4 md:basis-1/2 lg:basis-1/4">
            <div className="p-1">
              <Link href={`/products/${product.productSlug || product.productId}`}>
                <Card className="overflow-hidden border-none shadow-none h-full group relative">
                  <CardContent className="p-0">
                    <div className="aspect-[3/4] relative overflow-hidden rounded-lg bg-muted">
                      <Image
                        src={product.productImageUrl.startsWith("http") ? product.productImageUrl : `${process.env.NEXT_PUBLIC_BACKEND_URL || "http://localhost:5248"}${product.productImageUrl}`}
                        alt={product.productName}
                        fill
                        className="object-cover transition-transform duration-300 group-hover:scale-105"
                        sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"
                      />
                      {}
                      <div className="absolute inset-x-0 bottom-4 px-4 opacity-0 transition-opacity duration-300 group-hover:opacity-100 flex justify-center z-10">
                        <AddToCartButton product={product} variant="overlay" />
                      </div>
                    </div>
                    <div className="pt-4">
                      <h3 className="text-sm font-medium">{product.productName}</h3>
                      <p className="text-sm text-muted-foreground">${product.productPrice.toFixed(2)}</p>
                    </div>
                  </CardContent>
                </Card>
              </Link>
            </div>
          </CarouselItem>
        ))}
      </CarouselContent>
      <CarouselPrevious className="hidden md:flex" />
      <CarouselNext className="hidden md:flex" />
    </Carousel>
  );
}





function FeaturedSkeleton() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      {[...Array(4)].map((_, i) => (
        <div key={i} className="p-1">
          <div className="aspect-[3/4] rounded-lg bg-muted animate-pulse" />
          <div className="pt-4 space-y-2">
            <div className="h-4 bg-muted rounded animate-pulse w-3/4" />
            <div className="h-4 bg-muted rounded animate-pulse w-1/4" />
          </div>
        </div>
      ))}
    </div>
  );
}





export default function Home() {
  const [products, setProducts] = useState<Product[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function loadProducts() {
      try {
        const data = await getProducts();
        setProducts(data);
      } catch (error) {
        console.error("Failed to fetch products:", error);
      } finally {
        setIsLoading(false);
      }
    }

    loadProducts();
  }, []);

  return (
    <div className="flex flex-col gap-10 pb-10">
      {}
      <div className="border-b bg-muted/50 py-2">
        <Marquee className="max-w-full" pauseOnHover>
          <span className="mx-4 text-sm font-medium">FREE SHIPPING ON ALL ORDERS OVER $100</span>
          <span className="mx-4 text-sm font-medium">•</span>
          <span className="mx-4 text-sm font-medium">NEW COLLECTION DROPPING SOON</span>
          <span className="mx-4 text-sm font-medium">•</span>
          <span className="mx-4 text-sm font-medium">LIMITED TIME OFFER: 20% OFF</span>
          <span className="mx-4 text-sm font-medium">•</span>
        </Marquee>
      </div>

      {}
      <section className="container py-10 text-center md:py-20">
        <h1 className="text-4xl font-extrabold tracking-tight lg:text-5xl">
          ESSENTIALS
        </h1>
        <p className="mx-auto mt-4 max-w-xl text-muted-foreground">
          Premium minimalism for the modern individual. Quality materials, timeless design.
        </p>
        <div className="mt-8 flex justify-center gap-4">
          <Button size="lg" asChild>
            <Link href="/products">Shop Now</Link>
          </Button>
          <Button variant="outline" size="lg" asChild>
            <Link href="/products">View Collection</Link>
          </Button>
        </div>
      </section>

      {}
      <section className="container py-10">
        <div className="mb-8 flex items-center justify-between">
          <h2 className="text-2xl font-bold tracking-tight">Featured Products</h2>
          <Button variant="link" asChild>
            <Link href="/products">View All</Link>
          </Button>
        </div>

        {isLoading ? (
          <FeaturedSkeleton />
        ) : (
          <FeaturedCarousel products={products} isLoading={isLoading} />
        )}
      </section>

      {}
      <div className="border-t border-b bg-black text-white py-4 mt-10">
        <Marquee className="max-w-full" reverse pauseOnHover>
          <span className="mx-8 text-lg font-bold">MINIMAL STYLE</span>
          <span className="mx-8 text-lg font-bold">PREMIUM QUALITY</span>
          <span className="mx-8 text-lg font-bold">SUSTAINABLE MATERIALS</span>
          <span className="mx-8 text-lg font-bold">TIMELESS DESIGN</span>
        </Marquee>
      </div>
    </div>
  )
}
