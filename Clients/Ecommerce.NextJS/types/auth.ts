


export interface SignInDto {
    username: string;
    password: string;
}


export interface TokenResponse {
    access_token: string;
    refresh_token: string;
    expires_in: number;
    token_type: string;
    scope?: string;
}


export interface UserInfo {
    sub: string;           
    name?: string;         
    email?: string;        
    role?: string | string[]; 
    [key: string]: unknown; 
}


export interface AuthState {
    user: UserInfo | null;
    accessToken: string | null;
    refreshToken: string | null;
    expiresAt: Date | null;
    isAuthenticated: boolean;
    isLoading: boolean;
}


export interface LoginResult {
    success: boolean;
    error?: string;
    user?: UserInfo;
}


export interface DiscoveryDocument {
    token_endpoint: string;
    userinfo_endpoint: string;
    authorization_endpoint: string;
    end_session_endpoint: string;
    revocation_endpoint?: string;
}
