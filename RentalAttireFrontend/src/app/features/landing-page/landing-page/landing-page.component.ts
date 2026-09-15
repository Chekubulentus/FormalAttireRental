import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ClotheService } from '../../admin/clothes/clothe-service/clothe.service';
// Path assumed to mirror ClotheService's folder convention — adjust if CategoryService lives elsewhere.
import { CategoryService } from '../../admin/categories/category-service/category.service';
import { ClotheDTO } from '../../../data/models/DTOs/Clothes/clothes';
import { Category } from '../../../data/models/DTOs/Category/category';

type CategoryIcon = 'barong' | 'gown' | 'suit' | 'accessories';

interface CategoryShortcut {
  label: string;
  icon: CategoryIcon;
  routerLink: string;
}

interface StatItem {
  label: string;
  value: string;
}

interface GenderFilterOption {
  label: string;
  value: string; // '' = All
}

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent implements OnInit {

  constructor(
    private clotheService: ClotheService,
    private categoryService: CategoryService
  ) {}

  readonly categoryShortcuts: CategoryShortcut[] = [
    { label: 'Barongs', icon: 'barong', routerLink: '/customer/browse' },
    { label: 'Gowns', icon: 'gown', routerLink: '/customer/browse' },
    { label: 'Suits & Tuxedos', icon: 'suit', routerLink: '/customer/browse' },
    { label: 'Accessories', icon: 'accessories', routerLink: '/customer/browse' }
  ];

  readonly brandValues = [
    {
      title: 'Curated Quality',
      description: 'Every piece is inspected and cleaned after each rental, so what you wear always looks its first-time best.'
    },
    {
      title: 'Fair, Upfront Pricing',
      description: 'One rental price, one refundable deposit, no surprise fees at pickup.'
    },
    {
      title: 'Reserve With Ease',
      description: 'Pick your own dates, confirm with GCash, and track your rental status from browse to return.'
    }
  ];

  // PLACEHOLDER VALUES. Once GET /api/Stats/landing exists, replace this static
  // array with a fetched StatsDTO mapped into the same { label, value } shape.
  readonly stats: StatItem[] = [
    { label: 'Happy Customers', value: '500+' },
    { label: 'Rentals Completed', value: '1,200+' },
    { label: 'Curated Pieces', value: '300+' },
    { label: 'Categories', value: '4' }
  ];

  readonly genderOptions: GenderFilterOption[] = [
    { label: 'All', value: '' },
    { label: 'Male', value: 'Male' },
    { label: 'Female', value: 'Female' },
    { label: 'Unisex', value: 'Unisex' },
  ];

  categories: Category[] = [];
  selectedGender = '';
  selectedCategory = ''; // category name, '' = All

  popularClothes: ClotheDTO[] = [];
  isLoadingPopular = false;
  loadFailed = false;

  private readonly popularItemsToFetch = 12;
  // Below this many results, a looping scroll looks broken — show a static row instead.
  private readonly minItemsToAnimate = 3;

  /** True while the marquee is hovered — pauses the auto-scroll. */
  isPaused = false;

  ngOnInit(): void {
    this.loadCategories();
    this.loadPopularClothes();
  }

  async loadCategories(): Promise<void> {
    const result = await this.categoryService.getAllCategories();
    if (result.isSuccess && result.data) {
      this.categories = result.data;
    }
    // Silent fail here is fine — worst case the Category pill row just stays at "All".
  }

  async loadPopularClothes(): Promise<void> {
    this.isLoadingPopular = true;
    this.loadFailed = false;

    // Condition param intentionally left as '' — no condition filtering on this public section.
    const result = await this.clotheService.filterClothesAsync(
      '',
      '',
      this.selectedGender,
      this.selectedCategory,
      1,
      this.popularItemsToFetch
    );

    if (result.isSuccess && result.data) {
      this.popularClothes = result.data.items ?? [];
    } else {
      this.popularClothes = [];
      this.loadFailed = true;
    }

    this.isLoadingPopular = false;
  }

  onGenderFilterChange(value: string): void {
    if (this.selectedGender === value) return;
    this.selectedGender = value;
    this.loadPopularClothes();
  }

  onCategoryFilterChange(value: string): void {
    if (this.selectedCategory === value) return;
    this.selectedCategory = value;
    this.loadPopularClothes();
  }

  get shouldAnimate(): boolean {
    return this.popularClothes.length >= this.minItemsToAnimate;
  }

  /** Duplicated so the CSS marquee loop is seamless — only when actually animating. */
  get marqueeClothes(): ClotheDTO[] {
    return this.shouldAnimate
      ? [...this.popularClothes, ...this.popularClothes]
      : this.popularClothes;
  }

  /** Slower scroll for larger sets so the pace feels consistent regardless of item count. */
  get marqueeDurationSeconds(): number {
    return this.popularClothes.length * 4;
  }

  onImageError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}