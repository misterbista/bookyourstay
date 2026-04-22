import type { IconSvgElement } from "@hugeicons/react"

export type Destination = {
  name: string
  tagline: string
  image: string
  properties: number
}

export type Stay = {
  name: string
  location: string
  image: string
  price: number
  rating: number
  reviews: number
  type: string
  featured?: boolean
}

export type Category = {
  name: string
  icon: IconSvgElement
  count: number
}

export type SearchCriteria = {
  destination: string
  checkIn?: Date
  checkOut?: Date
  guests: number
}
