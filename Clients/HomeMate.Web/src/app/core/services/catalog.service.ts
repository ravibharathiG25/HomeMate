import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, ServiceItem } from '../models/catalog.models';

@Injectable({
  providedIn: 'root'
})
export class CatalogService {
  private readonly http = inject(HttpClient);

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>('/api/categories');
  }

  getCategoryBySlug(slug: string): Observable<Category & { services: ServiceItem[] }> {
    return this.http.get<Category & { services: ServiceItem[] }>(`/api/categories/${slug}`);
  }

  getServices(filter?: { categoryId?: string; featured?: boolean; search?: string }): Observable<ServiceItem[]> {
    let params = new HttpParams();
    if (filter?.categoryId) {
      params = params.set('categoryId', filter.categoryId);
    }
    if (filter?.featured !== undefined) {
      params = params.set('featured', filter.featured);
    }
    if (filter?.search) {
      params = params.set('search', filter.search);
    }

    return this.http.get<ServiceItem[]>('/api/services', { params });
  }

  getFeaturedServices(): Observable<ServiceItem[]> {
    return this.http.get<ServiceItem[]>('/api/services/featured');
  }

  getServiceById(id: string): Observable<ServiceItem> {
    return this.http.get<ServiceItem>(`/api/services/${id}`);
  }

  getServiceBySlug(slug: string): Observable<ServiceItem> {
    return this.http.get<ServiceItem>(`/api/services/slug/${slug}`);
  }
}
