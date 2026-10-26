import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking, CreateBookingRequest } from '../models/booking.models';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private readonly http = inject(HttpClient);

  createBooking(request: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>('/api/bookings', request);
  }

  getMyBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>('/api/bookings/my-bookings');
  }

  getBookingById(id: string): Observable<Booking> {
    return this.http.get<Booking>(`/api/bookings/${id}`);
  }
}
