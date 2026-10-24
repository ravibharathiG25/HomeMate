export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: 'Customer' | 'Provider' | 'Admin';
}

export interface AuthResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles?: string[];
  token?: string;
  accessToken?: string;
  refreshToken: string;
  refreshTokenExpiration?: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  errors?: Record<string, string[]> | string[];
}
