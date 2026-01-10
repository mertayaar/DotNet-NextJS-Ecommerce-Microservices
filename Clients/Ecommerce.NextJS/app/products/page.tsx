"use client";



import { useState, useEffect } from "react";
import ProductList from "@/components/ProductList";
import { getProducts, fetchCategories, CategoryDto } from "@/services/api";
import { Product } from "@/types/product";
import { Loader2 } from "lucide-react";





interface CategoryItem {
    id: string;
    name: string;
}





interface CategorySidebarProps {
    categories: CategoryItem[];
    selectedCategory: string;
    onSelectCategory: (categoryId: string) => void;
    productCounts: Record<string, number>;
    isLoading: boolean;
}

function CategorySidebar({
    categories,
    selectedCategory,
    onSelectCategory,
    productCounts,
    isLoading,
}: CategorySidebarProps) {
    return (
        <aside className="w-full md:w-[240px] shrink-0">
            <div className="sticky top-20 space-y-6">
                <div>
                    <h3 className="font-semibold mb-4">Categories</h3>
                    {isLoading ? (
                        <div className="flex items-center gap-2 text-muted-foreground">
                            <Loader2 className="h-4 w-4 animate-spin" />
                            <span className="text-sm">Loading...</span>
                        </div>
                    ) : (
                        <div className="space-y-2">
                            {categories.map((category) => (
                                <button
                                    key={category.id}
                                    onClick={() => onSelectCategory(category.id)}
                                    className={`block text-sm transition-colors hover:text-foreground w-full text-left ${selectedCategory === category.id
                                        ? "font-medium text-foreground"
                                        : "text-muted-foreground"
                                        }`}
                                >
                                    {category.name}
                                    {productCounts[category.id] !== undefined && (
                                        <span className="ml-2 text-xs text-muted-foreground">
                                            ({productCounts[category.id]})
                                        </span>
                                    )}
                                </button>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </aside>
    );
}





export default function ProductsPage() {
    
    
    

    const [selectedCategory, setSelectedCategory] = useState("all");
    const [categories, setCategories] = useState<CategoryItem[]>([{ id: "all", name: "All Products" }]);
    const [productCounts, setProductCounts] = useState<Record<string, number>>({});
    const [isLoadingCategories, setIsLoadingCategories] = useState(true);

    
    
    

    
    useEffect(() => {
        async function loadCategories() {
            try {
                const fetchedCategories = await fetchCategories();

                
                const mappedCategories: CategoryItem[] = [
                    { id: "all", name: "All Products" },
                    ...fetchedCategories.map((cat: CategoryDto) => ({
                        id: cat.categoryId,
                        name: cat.categoryName,
                    })),
                ];

                setCategories(mappedCategories);
            } catch (err) {
                console.error("Failed to fetch categories:", err);
            } finally {
                setIsLoadingCategories(false);
            }
        }

        loadCategories();
    }, []);

    
    useEffect(() => {
        async function calculateCounts() {
            try {
                const products = await getProducts();

                const counts: Record<string, number> = {
                    all: products.length,
                };

                products.forEach((product: Product) => {
                    counts[product.categoryId] = (counts[product.categoryId] || 0) + 1;
                });

                setProductCounts(counts);
            } catch (err) {
                console.error("Failed to calculate product counts:", err);
            }
        }

        calculateCounts();
    }, []);

    
    
    

    return (
        <div className="container py-10">
            <h1 className="text-3xl font-bold tracking-tight mb-8">Shop All</h1>

            <div className="flex flex-col md:flex-row gap-8">
                {}
                <CategorySidebar
                    categories={categories}
                    selectedCategory={selectedCategory}
                    onSelectCategory={setSelectedCategory}
                    productCounts={productCounts}
                    isLoading={isLoadingCategories}
                />

                {}
                <div className="flex-1">
                    <ProductList category={selectedCategory} />
                </div>
            </div>
        </div>
    );
}
