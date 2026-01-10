

const BFF_URL = '/backend';





export interface UserInfo {
    sub: string;
    name?: string;
    username?: string;
    email?: string;
    emailVerified?: boolean;
    roles?: string[];
    givenName?: string;
    familyName?: string;
    picture?: string;
}

export interface AuthResult {
    success: boolean;
    message?: string;
    returnUrl?: string;
}

export interface RegisterData {
    username: string;
    email: string;
    name: string;
    surname: string;
    password: string;
}






export async function loginWithCredentials(
    username: string,
    password: string,
    returnUrl: string = '/'
): Promise<AuthResult> {
    try {
        const response = await fetch(`${BFF_URL}/auth/login-credentials`, {
            method: 'POST',
            credentials: 'include', 
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, password, returnUrl }),
        });

        const data = await response.json();

        if (response.ok && data.success) {
            return {
                success: true,
                message: data.message || 'Login successful',
                returnUrl: data.returnUrl || returnUrl
            };
        }

        return {
            success: false,
            message: data.message || 'Invalid credentials'
        };
    } catch (error) {
        console.error('Login error:', error);
        return {
            success: false,
            message: 'Network error. Please try again.'
        };
    }
}


export function loginWithRedirect(returnUrl: string = '/'): void {
    if (typeof window === 'undefined') return;

    const loginUrl = `${BFF_URL}/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`;
    window.location.href = loginUrl;
}


export async function register(data: RegisterData): Promise<AuthResult> {
    try {
        const response = await fetch(`${BFF_URL}/auth/register`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        const result = await response.json();

        if (response.ok && result.success) {
            return { success: true, message: result.message || 'Registration successful' };
        }

        return {
            success: false,
            message: result.message || result.details || 'Registration failed'
        };
    } catch (error) {
        console.error('Register error:', error);
        return { success: false, message: 'Network error. Please try again.' };
    }
}


export async function logout(): Promise<void> {
    try {
        await fetch(`${BFF_URL}/auth/logout`, {
            method: 'POST',
            credentials: 'include',
        });
    } catch (error) {
        console.error('Logout error:', error);
    }
}


export async function getCurrentUser(): Promise<UserInfo | null> {
    try {
        const response = await fetch(`${BFF_URL}/auth/user`, {
            method: 'GET',
            credentials: 'include',
        });

        if (!response.ok) {
            return null;
        }

        const data = await response.json();
        return data.user || null;
    } catch (error) {
        console.error('Get user error:', error);
        return null;
    }
}


export async function checkAuthStatus(): Promise<{
    isAuthenticated: boolean;
    tokenExpired?: boolean;
}> {
    try {
        const response = await fetch(`${BFF_URL}/auth/status`, {
            method: 'GET',
            credentials: 'include',
        });

        if (!response.ok) {
            return { isAuthenticated: false };
        }

        return await response.json();
    } catch (error) {
        return { isAuthenticated: false };
    }
}


export async function updateProfile(data: { name: string; surname: string; email: string }): Promise<AuthResult> {
    try {
        const response = await fetch(`${BFF_URL}/users/profile`, {
            method: 'PUT',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        const result = await response.json();

        if (response.ok) {
            return { success: true, message: result.message || 'Profile updated successfully' };
        }

        return {
            success: false,
            message: result.message || 'Update failed'
        };
    } catch (error) {
        console.error('Update profile error:', error);
        return { success: false, message: 'Network error' };
    }
}


export async function changePassword(data: { currentPassword: string; newPassword: string }): Promise<AuthResult> {
    try {
        const response = await fetch(`${BFF_URL}/users/password`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        const result = await response.json();

        if (response.ok) {
            return { success: true, message: result.message || 'Password changed successfully' };
        }

        return {
            success: false,
            message: result.message || result.details || 'Password change failed'
        };
    } catch (error) {
        console.error('Change password error:', error);
        return { success: false, message: 'Network error' };
    }
}


export async function refreshSession(): Promise<boolean> {
    try {
        const response = await fetch(`${BFF_URL}/auth/refresh`, {
            method: 'POST',
            credentials: 'include',
        });

        return response.ok;
    } catch (error) {
        return false;
    }
}
