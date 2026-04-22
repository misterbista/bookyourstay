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

export type SearchCriteria = {
  destination: string
  checkIn?: Date
  checkOut?: Date
  guests: number
}
