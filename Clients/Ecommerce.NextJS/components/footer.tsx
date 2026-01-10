"use client";

import Link from "next/link"
import { useState, useEffect } from "react"
import { fetchCategories, CategoryDto } from "@/services/api"

export function Footer() {
    const [categories, setCategories] = useState<CategoryDto[]>([]);

    useEffect(() => {
        async function loadCategories() {
            try {
                const data = await fetchCategories();
                setCategories(data.slice(0, 2));
            } catch (err) {
                console.error("Failed to fetch categories for footer:", err);
            }
        }
        loadCategories();
    }, []);

    return (
        <footer className="border-t bg-muted/40">
            <div className="container py-10 md:py-16">
                <div className="grid grid-cols-2 gap-8 md:grid-cols-4 lg:grid-cols-5">
                    <div className="col-span-2 lg:col-span-2">
                        <Link href="/" className="text-xl font-bold tracking-widest uppercase">
                            Minimal Store
                        </Link>
                        <p className="mt-4 text-sm text-muted-foreground max-w-xs">
                            Premium minimalism for the modern individual. Quality materials, timeless design, and sustainable practices.
                        </p>
                    </div>

                    <div className="flex flex-col gap-2">
                        <h3 className="font-semibold">Shop</h3>
                        <Link href="/products" className="text-sm text-muted-foreground hover:text-foreground">All Products</Link>
                        {categories.map((cat) => (
                            <Link
                                key={cat.categoryId}
                                href={`/products?category=${cat.categoryId}`}
                                className="text-sm text-muted-foreground hover:text-foreground"
                            >
                                {cat.categoryName}
                            </Link>
                        ))}
                        <Link href="/products?featured=true" className="text-sm text-muted-foreground hover:text-foreground">Featured</Link>
                    </div>

                    <div className="flex flex-col gap-2">
                        <h3 className="font-semibold">Company</h3>
                        <Link href="/about" className="text-sm text-muted-foreground hover:text-foreground">About Us</Link>
                        <Link href="/contact" className="text-sm text-muted-foreground hover:text-foreground">Contact</Link>
                        <Link href="/careers" className="text-sm text-muted-foreground hover:text-foreground">Careers</Link>
                        <Link href="/privacy" className="text-sm text-muted-foreground hover:text-foreground">Privacy Policy</Link>
                    </div>

                    <div className="flex flex-col gap-2">
                        <h3 className="font-semibold">Support</h3>
                        <Link href="/faq" className="text-sm text-muted-foreground hover:text-foreground">FAQ</Link>
                        <Link href="/shipping" className="text-sm text-muted-foreground hover:text-foreground">Shipping & Returns</Link>
                        <Link href="/orders" className="text-sm text-muted-foreground hover:text-foreground">Order Status</Link>
                    </div>
                </div>

                <div className="mt-10 border-t pt-6 flex flex-col md:flex-row justify-between items-center gap-4">
                    <p className="text-xs text-muted-foreground">
                        © {new Date().getFullYear()} <a href="https://github.com/mertayaar" target="_blank" rel="noopener noreferrer" className="hover:underline">Mert Ayar</a>. All rights reserved.
                    </p>
                    <div className="flex gap-4">
                        { }
                    </div>
                </div>
            </div>
        </footer>
    )
}
