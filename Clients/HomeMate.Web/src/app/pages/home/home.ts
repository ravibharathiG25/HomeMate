import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

interface ServiceItem {
  id: string;
  name: string;
  category: string;
  description: string;
  rating: number;
  startingPrice: number;
  icon: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent {
  readonly authService = inject(AuthService);

  featuredServices: ServiceItem[] = [
    {
      id: 'plumbing',
      name: 'Expert Plumbing & Pipe Repair',
      category: 'Plumbing',
      description: 'Leak detection, pipe repairs, faucet installation, and drain unclogging.',
      rating: 4.9,
      startingPrice: 49,
      icon: 'plumbing'
    },
    {
      id: 'electrical',
      name: 'Licensed Electrical Services',
      category: 'Electrical',
      description: 'Wiring inspections, fixture installations, panel upgrades, and smart home setups.',
      rating: 4.95,
      startingPrice: 59,
      icon: 'electrical'
    },
    {
      id: 'cleaning',
      name: 'Deep Villa & Home Cleaning',
      category: 'Cleaning',
      description: 'Comprehensive sanitization, carpet shampooing, kitchen degreasing, and window shine.',
      rating: 4.88,
      startingPrice: 79,
      icon: 'cleaning'
    },
    {
      id: 'ac',
      name: 'AC Maintenance & Cooling Tune-up',
      category: 'HVAC',
      description: 'Filter cleaning, refrigerant refills, duct sterilization, and thermostat repairs.',
      rating: 4.92,
      startingPrice: 65,
      icon: 'ac'
    },
    {
      id: 'painting',
      name: 'Interior & Exterior Painting',
      category: 'Painting',
      description: 'Flawless wall finishes, moisture-resistant coatings, accent walls, and color consultations.',
      rating: 4.85,
      startingPrice: 120,
      icon: 'painting'
    },
    {
      id: 'carpentry',
      name: 'Custom Carpentry & Furniture Repair',
      category: 'Carpentry',
      description: 'Cabinetry, door alignments, bespoke shelving, and wooden deck refurbishing.',
      rating: 4.91,
      startingPrice: 55,
      icon: 'carpentry'
    }
  ];
}
