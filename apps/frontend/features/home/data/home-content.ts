import {
  Beach02Icon,
  Building03Icon,
  Home04Icon,
  Hotel01Icon,
  MapsGlobal01Icon,
} from "@hugeicons/core-free-icons"

const DESTINATIONS = [
  {
    name: "Bali, Indonesia",
    tagline: "Tropical paradise",
    image: "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=600&h=400&fit=crop",
    properties: 2340,
  },
  {
    name: "Santorini, Greece",
    tagline: "Sun-kissed shores",
    image: "https://images.unsplash.com/photo-1570077188670-e3a8d69ac5ff?w=600&h=400&fit=crop",
    properties: 1820,
  },
  {
    name: "Kyoto, Japan",
    tagline: "Ancient meets modern",
    image: "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=600&h=400&fit=crop",
    properties: 1450,
  },
  {
    name: "Amalfi Coast, Italy",
    tagline: "Coastal charm",
    image: "https://images.unsplash.com/photo-1534308983496-4fabb1a015ee?w=600&h=400&fit=crop",
    properties: 980,
  },
] as const

const FEATURED_STAYS = [
  {
    name: "The Coral Villa",
    location: "Bali, Indonesia",
    image: "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=600&h=400&fit=crop",
    price: 245,
    rating: 4.9,
    reviews: 128,
    type: "Villa",
    featured: true,
  },
  {
    name: "Azure Suites",
    location: "Santorini, Greece",
    image: "https://images.unsplash.com/photo-1602002418082-a4443e081dd1?w=600&h=400&fit=crop",
    price: 189,
    rating: 4.8,
    reviews: 96,
    type: "Hotel",
    featured: false,
  },
  {
    name: "Bamboo Retreat",
    location: "Kyoto, Japan",
    image: "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=600&h=400&fit=crop",
    price: 175,
    rating: 4.7,
    reviews: 214,
    type: "Ryokan",
    featured: true,
  },
  {
    name: "Cliffside Manor",
    location: "Amalfi Coast, Italy",
    image: "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=600&h=400&fit=crop",
    price: 320,
    rating: 4.9,
    reviews: 87,
    type: "Resort",
    featured: false,
  },
  {
    name: "Sunset Bungalow",
    location: "Maldives",
    image: "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=600&h=400&fit=crop",
    price: 410,
    rating: 5,
    reviews: 53,
    type: "Bungalow",
    featured: true,
  },
  {
    name: "Mountain Lodge",
    location: "Swiss Alps",
    image: "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=600&h=400&fit=crop",
    price: 199,
    rating: 4.6,
    reviews: 172,
    type: "Lodge",
    featured: false,
  },
] as const

const CATEGORIES = [
  { label: "All", icon: MapsGlobal01Icon },
  { label: "Hotels", icon: Hotel01Icon },
  { label: "Villas", icon: Home04Icon },
  { label: "Resorts", icon: Beach02Icon },
  { label: "Apartments", icon: Building03Icon },
] as const

const POPULAR_SEARCHES = ["Bali", "Santorini", "Maldives", "Swiss Alps"] as const
const NAV_LINKS = ["Explore", "Stays", "Experiences"] as const

export {
  CATEGORIES,
  DESTINATIONS,
  FEATURED_STAYS,
  NAV_LINKS,
  POPULAR_SEARCHES,
}
