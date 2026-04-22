import {
  Beach02Icon,
  Building03Icon,
  Home04Icon,
  Hotel01Icon,
  MapsGlobal01Icon,
} from "@hugeicons/core-free-icons"
import type { Category, Destination, Stay } from "../types/home"

export const DESTINATIONS: Destination[] = [
  {
    name: "Bali, Indonesia",
    tagline: "Tropical paradise",
    image:
      "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=600&h=400&fit=crop",
    properties: 2340,
  },
  {
    name: "Santorini, Greece",
    tagline: "Sun-kissed shores",
    image:
      "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=600&h=400&fit=crop",
    properties: 1820,
  },
  {
    name: "Kyoto, Japan",
    tagline: "Ancient meets modern",
    image:
      "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=600&h=400&fit=crop",
    properties: 1450,
  },
  {
    name: "Amalfi Coast, Italy",
    tagline: "Coastal charm",
    image:
      "https://images.unsplash.com/photo-1534308983496-4fabb1a015ee?w=600&h=400&fit=crop",
    properties: 980,
  },
] as const

export const FEATURED_STAYS: Stay[] = [
  {
    name: "The Coral Villa",
    location: "Bali, Indonesia",
    image:
      "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=600&h=400&fit=crop",
    price: 245,
    rating: 4.9,
    reviews: 128,
    type: "Villa",
    featured: true,
  },
  {
    name: "Azure Suites",
    location: "Santorini, Greece",
    image:
      "https://images.unsplash.com/photo-1602002418082-a4443e081dd1?w=600&h=400&fit=crop",
    price: 189,
    rating: 4.8,
    reviews: 95,
    type: "Hotel",
  },
  {
    name: "Mountain Retreat",
    location: "Swiss Alps, Switzerland",
    image:
      "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=600&h=400&fit=crop",
    price: 320,
    rating: 4.9,
    reviews: 67,
    type: "Chalet",
  },
  {
    name: "Urban Loft",
    location: "New York, USA",
    image:
      "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=600&h=400&fit=crop",
    price: 156,
    rating: 4.6,
    reviews: 203,
    type: "Apartment",
  },
  {
    name: "Desert Oasis",
    location: "Dubai, UAE",
    image:
      "https://images.unsplash.com/photo-1540541338287-41700207dee6?w=600&h=400&fit=crop",
    price: 412,
    rating: 4.9,
    reviews: 89,
    type: "Resort",
  },
  {
    name: "Countryside Cottage",
    location: "Cotswolds, UK",
    image:
      "https://images.unsplash.com/photo-1449844908441-8829872d2607?w=600&h=400&fit=crop",
    price: 134,
    rating: 4.7,
    reviews: 156,
    type: "Cottage",
  },
] as const

export const CATEGORIES: Category[] = [
  {
    name: "All",
    icon: MapsGlobal01Icon,
    count: 12450,
  },
  {
    name: "Beachfront",
    icon: Beach02Icon,
    count: 3240,
  },
  {
    name: "City Center",
    icon: Building03Icon,
    count: 5680,
  },
  {
    name: "Countryside",
    icon: Home04Icon,
    count: 1890,
  },
  {
    name: "Luxury",
    icon: Hotel01Icon,
    count: 1640,
  },
] as const

export const POPULAR_SEARCHES = [
  "Bali villas",
  "Paris apartments",
  "Tokyo hotels",
  "Santorini resorts",
  "New York lofts",
] as const
