import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent {
  readonly authService = inject(AuthService);

  testApiResult = signal<any | null>(null);
  testApiLoading = signal<boolean>(false);
  testApiError = signal<string | null>(null);

  testProtectedEndpoint(): void {
    this.testApiLoading.set(true);
    this.testApiError.set(null);
    this.testApiResult.set(null);

    this.authService.getMe().subscribe({
      next: (res) => {
        this.testApiLoading.set(false);
        this.testApiResult.set(res);
      },
      error: (err) => {
        this.testApiLoading.set(false);
        this.testApiError.set(err.message || 'Failed to call protected /api/Auth/me endpoint');
      }
    });
  }
}
