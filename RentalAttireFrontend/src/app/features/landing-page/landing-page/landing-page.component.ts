import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

interface FeaturedClothe {
  name: string;
  category: string;
  imageUrl: string;
  rentalPrice: number;
  depositAmount: number;
}

interface CategoryShortcut {
  label: string;
  imageUrl: string;
  routerLink: string;
}

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.scss'
})
export class LandingPageComponent {

  // Static placeholder data — replace with a real Clothe query
  // (e.g. a public "featured/best-rented" endpoint) once available.
  readonly categoryShortcuts: CategoryShortcut[] = [
    {
      label: 'Barongs',
      imageUrl: 'assets/images/landing/category-barong.jpg',
      routerLink: '/customer/browse'
    },
    {
      label: 'Gowns',
      imageUrl: 'assets/images/landing/category-gown.jpg',
      routerLink: '/customer/browse'
    },
    {
      label: 'Suits & Tuxedos',
      imageUrl: 'assets/images/landing/category-suit.jpg',
      routerLink: '/customer/browse'
    },
    {
      label: 'Accessories',
      imageUrl: 'assets/images/landing/category-accessories.jpg',
      routerLink: '/customer/browse'
    }
  ];

  readonly featuredClothes: FeaturedClothe[] = [
    {
      name: 'Classic Black Tuxedo',
      category: "Men's Formal",
      imageUrl: 'assets/images/landing/featured-tuxedo.jpg',
      rentalPrice: 1500,
      depositAmount: 2000
    },
    {
      name: 'Modern Barong Tagalog',
      category: "Men's Formal",
      imageUrl: 'assets/images/landing/featured-barong.jpg',
      rentalPrice: 1200,
      depositAmount: 1500
    },
    {
      name: 'Champagne Evening Gown',
      category: "Women's Formal",
      imageUrl: 'assets/images/landing/featured-gown.jpg',
      rentalPrice: 1800,
      depositAmount: 2500
    },
    {
      name: 'Slim-Fit Navy Suit',
      category: "Men's Formal",
      imageUrl: 'assets/images/landing/featured-navy-suit.jpg',
      rentalPrice: 1400,
      depositAmount: 2000
    }
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
}