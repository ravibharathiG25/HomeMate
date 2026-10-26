import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ProblemDetails, RegisterRequest } from '../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  registerData: RegisterRequest = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: 'Customer'
  };

  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  showPassword = signal<boolean>(false);

  setRole(role: 'Customer' | 'Provider'): void {
    this.registerData.role = role;
  }

  togglePasswordVisibility(): void {
    this.showPassword.update(v => !v);
  }

  onSubmit(): void {
    if (!this.registerData.firstName || !this.registerData.lastName || !this.registerData.email || !this.registerData.password) {
      this.errorMessage.set('Please complete all required fields.');
      return;
    }

    if (this.registerData.password.length < 6) {
      this.errorMessage.set('Password must be at least 6 characters long.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.authService.register(this.registerData).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.successMessage.set('Account created successfully! Redirecting you to sign in...');
        setTimeout(() => {
          this.router.navigate(['/login'], { queryParams: { registered: 'true' } });
        }, 1800);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (err.error) {
          const problem: ProblemDetails = err.error;
          if (problem.errors && typeof problem.errors === 'object' && !Array.isArray(problem.errors)) {
            const firstErrorKey = Object.keys(problem.errors)[0];
            this.errorMessage.set(problem.errors[firstErrorKey]?.[0] || problem.detail || 'Validation failed.');
          } else {
            this.errorMessage.set(problem.detail || problem.title || 'Registration failed. Please try again.');
          }
        } else {
          this.errorMessage.set('Unable to connect to the authentication service. Please ensure the API is running.');
        }
      }
    });
  }
}
