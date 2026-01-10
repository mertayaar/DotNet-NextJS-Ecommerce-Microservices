import { authenticatedFetch } from "./api";

export interface DiscountCoupon {
    couponCode: string;
    couponRate: number;
    isActive: boolean;
    validDate: string;
}

export async function getDiscountByCode(code: string): Promise<DiscountCoupon | null> {
    try {
        const response = await authenticatedFetch(`/api/discount/discounts/GetCodeDetailByCode?code=${code}`, {
            method: 'GET'
        });

        if (response.ok) {
            const result = await response.json();
            return result.data;
        }
        return null;
    } catch (error) {
        console.error("Error fetching discount code:", error);
        return null;
    }
}
