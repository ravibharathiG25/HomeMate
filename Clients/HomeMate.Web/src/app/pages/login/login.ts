import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LoginRequest, ProblemDetails } from '../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  credentials: LoginRequest = {
    email: '',
    password: ''
  };

  isLoading = false;
  errorMessage: string | null = null;
  showPassword = false;

  onSubmit(): void {
    if (!this.credentials.email || !this.credentials.password) {
      this.errorMessage = 'Please enter both your email and password.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        this.isLoading = false;
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error) {
          const problem: ProblemDetails = err.error;
          this.errorMessage = problem.detail || problem.title || 'Invalid email or password.';
        } else {
          this.errorMessage = 'Unable to connect to the authentication service. Please check if the API is running.';
        }
      }
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  fillDemo(role: 'customer' | 'provider'): void {
    if (role === 'customer') {
      this.credentials.email = 'customer@homemate.com';
      this.credentials.password = 'Password123!';
    } else {
      this.credentials.email = 'provider@homemate.com';
      this.credentials.password = 'Password123!';
    }
  }
}
