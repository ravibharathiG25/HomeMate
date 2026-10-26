export interface Category {
  id: string;
  name: string;
  slug: string;
  description: string;
  icon: string;
  imageUrl: string;
  displayOrder: number;
  servicesCount: number;
}

export interface ServiceItem {
  id: string;
  categoryId: string;
  categoryName: string;
  name: string;
  slug: string;
  shortDescription: string;
  description: string;
  price: number;
  originalPrice?: number | null;
  durationMinutes: number;
  imageUrl: string;
  rating: number;
  totalReviews: number;
  isFeatured: boolean;
}
