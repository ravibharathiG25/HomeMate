export interface CreateBookingRequest {
  serviceId: string;
  serviceName: string;
  scheduledDate: string;
  timeSlot: string;
  address: string;
  city: string;
  postalCode: string;
  customerPhone: string;
  notes?: string;
  price: number;
}

export interface Booking {
  id: string;
  bookingReference: string;
  customerId: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  serviceId: string;
  serviceName: string;
  scheduledDate: string;
  timeSlot: string;
  address: string;
  city: string;
  postalCode: string;
  notes?: string;
  price: number;
  tax: number;
  totalPrice: number;
  status: string | number;
  createdAtUtc: string;
}
