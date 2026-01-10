"use client";



import React, { createContext, useContext, useState, useEffect, useCallback, useRef } from 'react';
import {
    UserInfo,
    AuthResult,
    RegisterData,
    getCurrentUser,
    loginWithCredentials as performLogin,
    register as performRegister,
    logout as performLogout,
    checkAuthStatus,
    refreshSession
} from '@/services/authService';





interface AuthContextType {
    
    user: UserInfo | null;
    
    isAuthenticated: boolean;
    
    isLoading: boolean;
    
    error: string | null;
    
    login: (username: string, password: string, returnUrl?: string) => Promise<AuthResult>;
    
    register: (data: RegisterData) => Promise<AuthResult>;
    
    logout: () => Promise<void>;
    
    clearError: () => void;
    
    refreshAuth: () => Promise<void>;
}





const AuthContext = createContext<AuthContextType | undefined>(undefined);






const SESSION_REFRESH_INTERVAL = 10 * 60 * 1000;





interface AuthProviderProps {
    children: React.ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
    
    
    

    const [user, setUser] = useState<UserInfo | null>(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const refreshTimerRef = useRef<NodeJS.Timeout | null>(null);
    const isMountedRef = useRef(true);

    
    
    

    
    const initializeAuth = useCallback(async () => {
        try {
            const currentUser = await getCurrentUser();

            if (!isMountedRef.current) return;

            if (currentUser) {
                setUser(currentUser);
                setIsAuthenticated(true);
            } else {
                setUser(null);
                setIsAuthenticated(false);
            }
        } catch (err) {
            console.error('Failed to initialize auth:', err);
            if (isMountedRef.current) {
                setUser(null);
                setIsAuthenticated(false);
            }
        } finally {
            if (isMountedRef.current) {
                setIsLoading(false);
            }
        }
    }, []);

    
    const setupSessionRefresh = useCallback(() => {
        if (refreshTimerRef.current) {
            clearInterval(refreshTimerRef.current);
        }

        refreshTimerRef.current = setInterval(async () => {
            if (!isAuthenticated) return;

            const status = await checkAuthStatus();

            if (status.tokenExpired) {
                const refreshed = await refreshSession();
                if (!refreshed && isMountedRef.current) {
                    
                    setUser(null);
                    setIsAuthenticated(false);
                }
            }
        }, SESSION_REFRESH_INTERVAL);
    }, [isAuthenticated]);

    
    
    

    useEffect(() => {
        isMountedRef.current = true;
        initializeAuth();

        return () => {
            isMountedRef.current = false;
            if (refreshTimerRef.current) {
                clearInterval(refreshTimerRef.current);
            }
        };
    }, [initializeAuth]);

    useEffect(() => {
        if (isAuthenticated) {
            setupSessionRefresh();
        }
        return () => {
            if (refreshTimerRef.current) {
                clearInterval(refreshTimerRef.current);
            }
        };
    }, [isAuthenticated, setupSessionRefresh]);

    
    
    

    
    const login = useCallback(async (
        username: string,
        password: string,
        returnUrl: string = '/'
    ): Promise<AuthResult> => {
        setIsLoading(true);
        setError(null);

        try {
            const result = await performLogin(username, password, returnUrl);

            if (result.success) {
                
                const currentUser = await getCurrentUser();

                if (isMountedRef.current) {
                    setUser(currentUser);
                    setIsAuthenticated(true);
                }
            } else {
                if (isMountedRef.current) {
                    setError(result.message || 'Login failed');
                }
            }

            return result;
        } catch (err) {
            const message = 'An unexpected error occurred';
            if (isMountedRef.current) {
                setError(message);
            }
            return { success: false, message };
        } finally {
            if (isMountedRef.current) {
                setIsLoading(false);
            }
        }
    }, []);

    
    const register = useCallback(async (data: RegisterData): Promise<AuthResult> => {
        setIsLoading(true);
        setError(null);

        try {
            const result = await performRegister(data);

            if (!result.success && isMountedRef.current) {
                setError(result.message || 'Registration failed');
            }

            return result;
        } catch (err) {
            const message = 'An unexpected error occurred';
            if (isMountedRef.current) {
                setError(message);
            }
            return { success: false, message };
        } finally {
            if (isMountedRef.current) {
                setIsLoading(false);
            }
        }
    }, []);

    
    const logout = useCallback(async (): Promise<void> => {
        setIsLoading(true);

        try {
            await performLogout();
        } finally {
            if (isMountedRef.current) {
                setUser(null);
                setIsAuthenticated(false);
                setError(null);
                setIsLoading(false);
            }
        }
    }, []);

    
    const clearError = useCallback(() => {
        setError(null);
    }, []);

    
    const refreshAuth = useCallback(async () => {
        await initializeAuth();
    }, [initializeAuth]);

    
    
    

    const value: AuthContextType = {
        user,
        isAuthenticated,
        isLoading,
        error,
        login,
        register,
        logout,
        clearError,
        refreshAuth
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}






export function useAuth(): AuthContextType {
    const context = useContext(AuthContext);

    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }

    return context;
}
