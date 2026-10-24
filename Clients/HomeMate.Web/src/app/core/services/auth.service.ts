import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RefreshTokenRequest,
  RegisterRequest,
  User
} from '../models/auth.models';

const TOKEN_KEY = 'homemate_jwt_token';
const REFRESH_TOKEN_KEY = 'homemate_refresh_token';
const USER_KEY = 'homemate_user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly authUrl = `${environment.apiUrl}/Auth`;

  // Reactive State using Angular Signals
  readonly currentUser = signal<User | null>(this.getStoredUser());
  readonly token = signal<string | null>(this.getStoredToken());
  readonly refreshTokenValue = signal<string | null>(this.getStoredRefreshToken());

  // Computed Selectors
  readonly isAuthenticated = computed(() => !!this.token() && !!this.currentUser());
  readonly userRole = computed(() => this.currentUser()?.roles?.[0] ?? null);
  readonly isCustomer = computed(() => this.currentUser()?.roles?.includes('Customer') ?? false);
  readonly isProvider = computed(() => this.currentUser()?.roles?.includes('Provider') ?? false);
  readonly isAdmin = computed(() => this.currentUser()?.roles?.includes('Admin') ?? false);

  constructor() {}

  register(request: RegisterRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.authUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.authUrl}/login`, request).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const currentRefreshToken = this.refreshTokenValue();
    if (!currentRefreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token available.'));
    }

    const payload: RefreshTokenRequest = { refreshToken: currentRefreshToken };
    return this.http.post<AuthResponse>(`${this.authUrl}/refresh-token`, payload).pipe(
      tap(response => this.handleAuthSuccess(response)),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    const rToken = this.refreshTokenValue();
    if (rToken) {
      // Fire-and-forget revoke token on backend
      this.http.post(`${this.authUrl}/revoke-token`, { refreshToken: rToken }).subscribe({
        error: () => {} // Ignore errors on logout
      });
    }

    this.clearStorage();
    this.token.set(null);
    this.refreshTokenValue.set(null);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getMe(): Observable<any> {
    return this.http.get(`${this.authUrl}/me`);
  }

  private handleAuthSuccess(response: AuthResponse): void {
    const rawToken = response.accessToken || response.token || '';
    const roles = response.roles || [];

    const user: User = {
      id: response.id,
      firstName: response.firstName,
      lastName: response.lastName,
      email: response.email,
      roles: roles
    };

    localStorage.setItem(TOKEN_KEY, rawToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(user));

    this.token.set(rawToken);
    this.refreshTokenValue.set(response.refreshToken);
    this.currentUser.set(user);
  }

  private getStoredToken(): string | null {
    const val = localStorage.getItem(TOKEN_KEY);
    return val && val !== 'undefined' && val !== 'null' ? val : null;
  }

  private getStoredRefreshToken(): string | null {
    const val = localStorage.getItem(REFRESH_TOKEN_KEY);
    return val && val !== 'undefined' && val !== 'null' ? val : null;
  }

  private getStoredUser(): User | null {
    const userJson = localStorage.getItem(USER_KEY);
    if (!userJson || userJson === 'undefined' || userJson === 'null') return null;
    try {
      return JSON.parse(userJson) as User;
    } catch {
      return null;
    }
  }

  private clearStorage(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }
}
