import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ClotheService } from '../../admin/clothes/clothe-service/clothe.service';
import { ClotheDTO } from '../../../data/models/DTOs/Clothes/clothes';
import { CategoryService } from '../../admin/categories/category-service/category.service';
import { Category } from '../../../data/models/DTOs/Category/category';

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

  // ── Category filter chips ────────────────────────────────────────────────
  categories: Category[] = [];
  isLoadingCategories = true;
  /** Empty string = "All" (no category filter applied). */
  selectedCategory = '';

  // ── Clothes grid (single section, filtered by selectedCategory) ─────────
  clothes: ClotheDTO[] = [];
  isLoadingClothes = true;

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

  async ngOnInit(): Promise<void> {
    await Promise.all([
      this.loadCategories(),
      this.loadClothes()
    ]);
  }

  private async loadCategories(): Promise<void> {
    this.isLoadingCategories = true;

    const result = await this.categoryService.getAllCategories();

    if (result.isSuccess && result.data) {
      this.categories = result.data;
    }

    this.isLoadingCategories = false;
  }

  private async loadClothes(): Promise<void> {
    this.isLoadingClothes = true;

    // ASSUMPTION: fetching 10 items so the auto-scroll carousel has enough
    // content to loop without looking sparse. Adjust if your typical
    // per-category catalog size is smaller or larger than this.
    const result = await this.clotheService.filterClothesAsync(
      '', '', '', this.selectedCategory, 1, 10
    );

    if (result.isSuccess && result.data) {
      this.clothes = result.data.items;
    } else {
      this.clothes = [];
    }

    this.isLoadingClothes = false;
  }

  selectCategory(categoryName: string): void {
    if (this.selectedCategory === categoryName) return;
    this.selectedCategory = categoryName;
    this.loadClothes();
  }

  /**
   * Duplicating a very small result set (1–2 items) doesn't produce a real
   * loop — it just looks like the same card shown twice, which is exactly
   * the bug this guards against. Below this threshold, the track renders
   * once and holds still (see carousel__track--static in the SCSS) instead
   * of faking a loop with too little content.
   */
  get shouldLoop(): boolean {
    return this.clothes.length >= 4;
  }

  /**
   * The carousel renders this list, not `clothes` directly. When there's
   * enough content, it's the same items duplicated once so the CSS marquee
   * animation can loop seamlessly (see the SCSS carousel keyframes for how
   * the 0%/-50% duplication lines up). When there isn't enough content
   * (see shouldLoop), it's just the plain list, shown once.
   */
  get carouselClothes(): ClotheDTO[] {
    return this.shouldLoop ? [...this.clothes, ...this.clothes] : this.clothes;
  }
}