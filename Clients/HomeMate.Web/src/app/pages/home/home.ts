import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CatalogService } from '../../core/services/catalog.service';
import { Category, ServiceItem } from '../../core/models/catalog.models';
import { BookingModalComponent } from '../../components/booking-modal/booking-modal';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, BookingModalComponent],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent implements OnInit {
  readonly authService = inject(AuthService);
  private readonly catalogService = inject(CatalogService);

  categories = signal<Category[]>([]);
  services = signal<ServiceItem[]>([]);
  selectedCategoryId = signal<string | null>(null);
  selectedServiceForBooking = signal<ServiceItem | null>(null);
  isLoading = signal<boolean>(true);
  searchQuery = signal<string>('');

  // Real-time computed filtered services based on active category and live search query
  filteredServices = computed(() => {
    const list = this.services();
    const query = this.searchQuery().trim().toLowerCase();
    const catId = this.selectedCategoryId();

    return list.filter(item => {
      const matchesCategory = !catId || item.categoryId === catId;
      const matchesQuery = !query ||
        item.name.toLowerCase().includes(query) ||
        item.categoryName.toLowerCase().includes(query) ||
        (item.shortDescription && item.shortDescription.toLowerCase().includes(query));

      return matchesCategory && matchesQuery;
    });
  });

  openBookingModal(service: ServiceItem): void {
    this.selectedServiceForBooking.set(service);
  }

  closeBookingModal(): void {
    this.selectedServiceForBooking.set(null);
  }

  ngOnInit(): void {
    this.loadCatalogData();
  }

  loadCatalogData(): void {
    this.isLoading.set(true);

    // Load Categories
    this.catalogService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
      },
      error: (err) => {
        console.warn('Could not load categories from backend, using default fallback', err);
      }
    });

    // Load Services
    this.loadServices();
  }

  selectCategory(categoryId: string | null): void {
    this.selectedCategoryId.set(categoryId);
    this.loadServices(categoryId ?? undefined);
  }

  onSearchInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }

  setQuickSearch(term: string): void {
    this.searchQuery.set(term);
    this.selectedCategoryId.set(null);
  }

  clearSearch(): void {
    this.searchQuery.set('');
  }

  loadServices(categoryId?: string): void {
    this.isLoading.set(true);
    this.catalogService.getServices({ categoryId }).subscribe({
      next: (items) => {
        this.services.set(items);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.warn('Could not load services from backend, using fallback services', err);
        this.fallbackServices();
        this.isLoading.set(false);
      }
    });
  }

  calculateDiscount(price: number, originalPrice?: number | null): number {
    if (!originalPrice || originalPrice <= price) return 0;
    return Math.round(((originalPrice - price) / originalPrice) * 100);
  }

  private fallbackServices(): void {
    this.services.set([
      {
        id: '1',
        categoryId: 'clean',
        categoryName: 'Cleaning',
        name: 'Full Home Deep Sanitization & Cleaning',
        slug: 'full-home-deep-cleaning',
        shortDescription: 'Hospital-grade scrubbing, window polish, deep steam disinfection, and full sanitization for apartments up to 3BHK.',
        description: 'Complete floor scrubbing, window cleaning, dusting, and sanitization for up to 3BHK.',
        price: 129.99,
        originalPrice: 169.99,
        durationMinutes: 240,
        imageUrl: 'https://images.unsplash.com/photo-1581578731548-c64695cc6952?auto=format&fit=crop&w=800&q=80',
        rating: 4.95,
        totalReviews: 142,
        isFeatured: true
      },
      {
        id: '2',
        categoryId: 'plumb',
        categoryName: 'Plumbing',
        name: 'Precision Pipe & Fixture Leak Solution',
        slug: 'tap-pipe-leak-repair',
        shortDescription: 'Master diagnostic & repair for concealed wall leaks, luxury mixers, diverters, high-pressure valves, and drainage lines.',
        description: 'Quick fixing of leaking mixers, diverters, pipe connections, and valves.',
        price: 39.99,
        originalPrice: 55.00,
        durationMinutes: 45,
        imageUrl: 'https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&w=800&q=80',
        rating: 4.92,
        totalReviews: 238,
        isFeatured: true
      },
      {
        id: '3',
        categoryId: 'elec',
        categoryName: 'Electrical',
        name: 'Master Switchboard & MCB Troubleshooting',
        slug: 'switchboard-mcb-troubleshooting',
        shortDescription: 'Thermal analysis, replacement of sparking contactors, circuit breaker trip fixes, and smart surge protector setup.',
        description: 'Safe replacement of sparking switches, blown fuses, and tripping breakers.',
        price: 44.99,
        originalPrice: 65.00,
        durationMinutes: 60,
        imageUrl: 'https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&w=800&q=80',
        rating: 4.97,
        totalReviews: 189,
        isFeatured: true
      },
      {
        id: '4',
        categoryId: 'appliance',
        categoryName: 'Appliance Care',
        name: 'Inverter Split AC Hydro-Jet Servicing',
        slug: 'split-ac-power-jet-servicing',
        shortDescription: 'High-pressure foam hydro-jet wash of evaporator coils, antimicrobial blower wheel treatment, and refrigerant diagnostic check.',
        description: 'High-pressure jet wash of cooling coils, filter sanitization, and gas pressure check.',
        price: 69.99,
        originalPrice: 95.00,
        durationMinutes: 75,
        imageUrl: 'https://images.unsplash.com/photo-1621905251918-48416bd8575a?auto=format&fit=crop&w=800&q=80',
        rating: 4.96,
        totalReviews: 382,
        isFeatured: true
      },
      {
        id: '5',
        categoryId: 'paint',
        categoryName: 'Painting',
        name: 'Wall Dampness & Nano-Waterproofing System',
        slug: 'wall-dampness-treatment',
        shortDescription: 'Thermal moisture scan, deep crystalline injection barrier, and elastomeric waterproof shield against efflorescence.',
        description: 'Deep moisture barrier injection and elastomeric waterproofing coating.',
        price: 119.99,
        originalPrice: 159.00,
        durationMinutes: 180,
        imageUrl: 'https://images.unsplash.com/photo-1562259949-e8e7689d7828?auto=format&fit=crop&w=800&q=80',
        rating: 4.88,
        totalReviews: 76,
        isFeatured: true
      },
      {
        id: '6',
        categoryId: 'carpentry',
        categoryName: 'Carpentry',
        name: 'Biometric Smart Lock & Custom Door Fitting',
        slug: 'smart-door-lock-installation',
        shortDescription: 'Precision CNC chisel mortise installation for fingerprint, RFID, and digital smart door systems with zero cosmetic damage.',
        description: 'Precision chisel mortise fitting for digital fingerprint and keypad smart locks.',
        price: 49.99,
        originalPrice: 70.00,
        durationMinutes: 60,
        imageUrl: 'https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&w=800&q=80',
        rating: 4.94,
        totalReviews: 110,
        isFeatured: false
      }
    ]);
  }
}
