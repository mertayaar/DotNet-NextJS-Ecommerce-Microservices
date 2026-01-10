

const BFF_URL = '/backend';

export interface Address {
    id: number;
    title: string;
    name: string;
    surname: string;
    email: string;
    phone: string;
    city: string;
    district: string;
    addressLine: string;
    isDefault: boolean;
}

export interface CreateAddressData {
    title: string;
    name: string;
    surname: string;
    email: string;
    phone: string;
    city: string;
    district: string;
    addressLine: string;
    isDefault?: boolean;
}

export interface UpdateAddressData extends CreateAddressData { }


function mapCargoToAddress(cargo: any): Address {
    return {
        id: cargo.cargoCustomerId,
        title: cargo.title || '',
        name: cargo.name || '',
        surname: cargo.surname || '',
        email: cargo.email || '',
        phone: cargo.phone || '',
        city: cargo.city || '',
        district: cargo.district || '',
        addressLine: cargo.address || '',
        isDefault: cargo.isDefault || false
    };
}


function mapAddressToCargo(data: CreateAddressData | UpdateAddressData): any {
    return {
        title: data.title,
        name: data.name,
        surname: data.surname,
        email: data.email,
        phone: data.phone,
        city: data.city,
        district: data.district,
        address: data.addressLine,
        isDefault: data.isDefault
    };
}


export async function getAddresses(): Promise<Address[]> {
    try {
        const response = await fetch(`${BFF_URL}/addresses`, {
            method: 'GET',
            credentials: 'include',
        });

        if (!response.ok) {
            return [];
        }

        const data = await response.json();
        
        const addresses = Array.isArray(data) ? data : (data.data || data);
        return Array.isArray(addresses) ? addresses.map(mapCargoToAddress) : [];
    } catch (error) {
        console.error('Get addresses error:', error);
        return [];
    }
}


export async function getAddress(id: number): Promise<Address | null> {
    try {
        const response = await fetch(`${BFF_URL}/addresses/${id}`, {
            method: 'GET',
            credentials: 'include',
        });

        if (!response.ok) {
            return null;
        }

        const data = await response.json();
        return mapCargoToAddress(data);
    } catch (error) {
        console.error('Get address error:', error);
        return null;
    }
}


export async function createAddress(data: CreateAddressData): Promise<{ success: boolean; id?: number; message?: string }> {
    try {
        const response = await fetch(`${BFF_URL}/addresses`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(mapAddressToCargo(data)),
        });

        const result = await response.json();

        if (response.ok) {
            return { success: true, id: result.id };
        }

        return { success: false, message: result.error || 'Failed to create address' };
    } catch (error) {
        console.error('Create address error:', error);
        return { success: false, message: 'Network error' };
    }
}


export async function updateAddress(id: number, data: UpdateAddressData): Promise<{ success: boolean; message?: string }> {
    try {
        const response = await fetch(`${BFF_URL}/addresses/${id}`, {
            method: 'PUT',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(mapAddressToCargo(data)),
        });

        if (response.ok) {
            return { success: true };
        }

        const result = await response.json();
        return { success: false, message: result.error || 'Failed to update address' };
    } catch (error) {
        console.error('Update address error:', error);
        return { success: false, message: 'Network error' };
    }
}


export async function deleteAddress(id: number): Promise<{ success: boolean; message?: string }> {
    try {
        const response = await fetch(`${BFF_URL}/addresses/${id}`, {
            method: 'DELETE',
            credentials: 'include',
        });

        if (response.ok) {
            return { success: true };
        }

        const result = await response.json();
        return { success: false, message: result.error || 'Failed to delete address' };
    } catch (error) {
        console.error('Delete address error:', error);
        return { success: false, message: 'Network error' };
    }
}


export async function setDefaultAddress(id: number): Promise<{ success: boolean; message?: string }> {
    try {
        const response = await fetch(`${BFF_URL}/addresses/${id}/set-default`, {
            method: 'POST',
            credentials: 'include',
        });

        if (response.ok) {
            return { success: true };
        }

        const result = await response.json();
        return { success: false, message: result.error || 'Failed to set default address' };
    } catch (error) {
        console.error('Set default address error:', error);
        return { success: false, message: 'Network error' };
    }
}
