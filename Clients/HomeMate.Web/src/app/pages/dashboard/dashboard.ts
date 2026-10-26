import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { BookingService } from '../../core/services/booking.service';
import { Booking } from '../../core/models/booking.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  readonly authService = inject(AuthService);
  private readonly bookingService = inject(BookingService);

  bookings = signal<Booking[]>([]);
  isLoadingBookings = signal<boolean>(true);

  // Computed KPI Metrics
  activeBookings = computed(() =>
    this.bookings().filter(b => String(b.status).toLowerCase() !== 'completed' && String(b.status).toLowerCase() !== 'cancelled')
  );

  completedBookings = computed(() =>
    this.bookings().filter(b => String(b.status).toLowerCase() === 'completed')
  );

  totalSpent = computed(() =>
    this.bookings().reduce((sum, b) => sum + (b.totalPrice || 0), 0)
  );

  ngOnInit(): void {
    this.loadMyBookings();
  }

  loadMyBookings(): void {
    this.isLoadingBookings.set(true);
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.bookings.set(data);
        this.isLoadingBookings.set(false);
      },
      error: (err) => {
        console.warn('Could not load bookings', err);
        this.isLoadingBookings.set(false);
      }
    });
  }
}
