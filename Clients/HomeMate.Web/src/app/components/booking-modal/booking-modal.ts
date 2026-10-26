import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ServiceItem } from '../../core/models/catalog.models';
import { Booking, CreateBookingRequest } from '../../core/models/booking.models';
import { BookingService } from '../../core/services/booking.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-booking-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-modal.html',
  styleUrl: './booking-modal.css'
})
export class BookingModalComponent {
  private readonly bookingService = inject(BookingService);
  readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  @Input() service: ServiceItem | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() booked = new EventEmitter<Booking>();

  // Multi-step progress (1: Date, 2: Slot, 3: Address, 4: Summary, 5: Confirmed)
  currentStep = signal<number>(1);

  availableSlots: string[] = [
    '09:00 AM - 11:00 AM',
    '11:30 AM - 01:30 PM',
    '02:00 PM - 04:00 PM',
    '04:30 PM - 06:30 PM'
  ];

  selectedDate: string = this.getTomorrowDateString();
  selectedSlot: string = '09:00 AM - 11:00 AM';

  address: string = '';
  city: string = '';
  postalCode: string = '';
  phone: string = '+1 555-0142';
  notes: string = '';

  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successBooking = signal<Booking | null>(null);

  get taxAmount(): number {
    return Math.round((this.service?.price ?? 0) * 0.05 * 100) / 100;
  }

  get totalAmount(): number {
    return Math.round(((this.service?.price ?? 0) + this.taxAmount) * 100) / 100;
  }

  selectSlot(slot: string): void {
    this.selectedSlot = slot;
  }

  nextStep(): void {
    this.errorMessage.set(null);
    const step = this.currentStep();

    if (step === 1) {
      if (!this.selectedDate) {
        this.errorMessage.set('Please select an appointment date.');
        return;
      }
      this.currentStep.set(2);
    } else if (step === 2) {
      if (!this.selectedSlot) {
        this.errorMessage.set('Please choose an arrival time window.');
        return;
      }
      this.currentStep.set(3);
    } else if (step === 3) {
      if (!this.address.trim() || !this.city.trim() || !this.postalCode.trim() || !this.phone.trim()) {
        this.errorMessage.set('Please provide all address and contact details to proceed.');
        return;
      }
      this.currentStep.set(4);
    }
  }

  prevStep(): void {
    this.errorMessage.set(null);
    const step = this.currentStep();
    if (step > 1 && step < 5) {
      this.currentStep.set(step - 1);
    }
  }

  goToStep(stepNum: number): void {
    if (stepNum < this.currentStep() && this.currentStep() < 5) {
      this.errorMessage.set(null);
      this.currentStep.set(stepNum);
    }
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closeModal();
    }
  }

  closeModal(): void {
    this.close.emit();
  }

  confirmBooking(): void {
    if (!this.service) return;

    if (!this.authService.isAuthenticated()) {
      this.closeModal();
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/' } });
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const payload: CreateBookingRequest = {
      serviceId: this.service.id,
      serviceName: this.service.name,
      scheduledDate: new Date(this.selectedDate).toISOString(),
      timeSlot: this.selectedSlot,
      address: this.address.trim(),
      city: this.city.trim(),
      postalCode: this.postalCode.trim(),
      customerPhone: this.phone.trim(),
      notes: this.notes.trim() || undefined,
      price: this.service.price
    };

    this.bookingService.createBooking(payload).subscribe({
      next: (booking) => {
        this.isLoading.set(false);
        this.successBooking.set(booking);
        this.currentStep.set(5);
        this.booked.emit(booking);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.detail || err.error?.title || 'Failed to complete booking. Please try again.');
      }
    });
  }

  goToDashboard(): void {
    this.closeModal();
    this.router.navigate(['/dashboard']);
  }

  private getTomorrowDateString(): string {
    const d = new Date();
    d.setDate(d.getDate() + 1);
    return d.toISOString().split('T')[0];
  }
}
