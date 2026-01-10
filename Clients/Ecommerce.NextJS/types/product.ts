


export interface Product {
  productId: string;
  productName: string;
  productPrice: number;
  productImageUrl: string;
  productDescription: string;
  productSlug: string;
  categoryId: string;
}


export interface ProductDisplay {
  id: string;
  name: string;
  price: number;
  image: string;
  description: string;
  category: string;
}


export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T;
}


export interface ProductListResponse {
  products: Product[];
  totalCount?: number;
}


export interface Category {
  id: string;
  name: string;
}


export function mapProductToDisplay(product: Product): ProductDisplay {
  return {
    id: product.productId,
    name: product.productName,
    price: product.productPrice,
    image: product.productImageUrl,
    description: product.productDescription,
    category: product.categoryId,
  };
}
