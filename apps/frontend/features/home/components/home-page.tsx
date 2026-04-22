"use client"

import { useState } from "react"
import { HugeiconsIcon } from "@hugeicons/react"
import { ArrowRight01Icon, MapsGlobal01Icon } from "@hugeicons/core-free-icons"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { SiteFooter } from "@/shared/layout/site-footer"
import { SiteNavbar } from "@/shared/layout/site-navbar"
import { SearchBar } from "./search-bar"
import { DestinationCard } from "./destination-card"
import { StayCard } from "./stay-card"
import {
  DEMO_CATEGORIES,
  DEMO_DESTINATIONS,
  DEMO_FEATURED_STAYS,
  DEMO_POPULAR_SEARCHES,
} from "../data/home-demo-data"
import type { SearchCriteria } from "../types/home"

export function HomePage() {
  const [activeCategory, setActiveCategory] = useState("All")
  const [searchResults, setSearchResults] = useState<SearchCriteria | null>(
    null
  )

  const handleSearch = (searchData: SearchCriteria) => {
    setSearchResults(searchData)
  }

  return (
    <div className="min-h-svh bg-background text-foreground">
      <SiteNavbar />

      <section className="relative flex min-h-[85vh] flex-col items-center justify-center overflow-hidden px-4 pt-16">
        <div className="pointer-events-none absolute inset-0 overflow-hidden">
          <div className="absolute -top-32 -right-32 size-96 rounded-full bg-primary/5 blur-3xl" />
          <div className="absolute -bottom-32 -left-32 size-96 rounded-full bg-chart-1/5 blur-3xl" />
          <div className="absolute top-1/3 left-1/2 size-64 -translate-x-1/2 rounded-full bg-chart-2/5 blur-3xl" />
        </div>

        <div className="relative z-10 mx-auto flex w-full max-w-6xl flex-col items-center text-center">
          <Badge variant="secondary" className="mb-6">
            <HugeiconsIcon icon={MapsGlobal01Icon} size={12} strokeWidth={2} />
            Over 10,000+ stays worldwide
          </Badge>

          <h1 className="text-4xl leading-tight font-bold tracking-tight sm:text-5xl md:text-6xl">
            Find your perfect <span className="text-primary">getaway</span>
          </h1>

          <p className="mt-4 max-w-xl text-base text-muted-foreground sm:text-lg">
            Discover unique stays, boutique hotels, and curated experiences
            around the world - all in one place.
          </p>

          <div className="mt-10 flex w-full justify-center">
            <SearchBar onSearch={handleSearch} />
          </div>

          {searchResults && (
            <div className="mt-6 rounded-lg bg-muted/50 p-4 text-left">
              <h3 className="font-semibold">Search Results:</h3>
              <p className="text-sm text-muted-foreground">
                Destination: {searchResults.destination || "Any"}
                {searchResults.checkIn &&
                  ` | Check-in: ${searchResults.checkIn.toLocaleDateString()}`}
                {searchResults.checkOut &&
                  ` | Check-out: ${searchResults.checkOut.toLocaleDateString()}`}
                {` | Guests: ${searchResults.guests}`}
              </p>
            </div>
          )}

          <div className="mt-6 flex flex-wrap items-center justify-center gap-2">
            <span className="text-xs text-muted-foreground">Popular:</span>
            {DEMO_POPULAR_SEARCHES.map((place) => (
              <Button
                key={place}
                variant="outline"
                size="sm"
                className="rounded-full text-xs"
                onClick={() => handleSearch({ destination: place, guests: 1 })}
              >
                {place}
              </Button>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <div className="flex items-center gap-2 overflow-x-auto pb-2">
          {DEMO_CATEGORIES.map((cat) => (
            <button
              key={cat.name}
              onClick={() => setActiveCategory(cat.name)}
              className={cn(
                "flex shrink-0 items-center gap-2 rounded-full border px-4 py-2 text-sm font-medium transition-all",
                activeCategory === cat.name
                  ? "border-primary bg-primary/10 text-primary"
                  : "border-border bg-card text-muted-foreground hover:border-primary/30 hover:text-foreground"
              )}
            >
              <HugeiconsIcon icon={cat.icon} size={16} strokeWidth={1.8} />
              {cat.name}
            </button>
          ))}
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
        <div className="mb-8 flex items-end justify-between">
          <div>
            <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">
              Trending destinations
            </h2>
            <p className="mt-1 text-sm text-muted-foreground">
              Most popular places to stay right now
            </p>
          </div>
          <Button variant="ghost" className="hidden gap-1 sm:inline-flex">
            View all
            <HugeiconsIcon icon={ArrowRight01Icon} size={14} strokeWidth={2} />
          </Button>
        </div>

        <div className="grid grid-cols-2 gap-4 sm:gap-6 lg:grid-cols-4">
          {DEMO_DESTINATIONS.map((dest) => (
            <DestinationCard key={dest.name} destination={dest} />
          ))}
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 pb-20 sm:px-6 lg:px-8">
        <div className="mb-8 flex items-end justify-between">
          <div>
            <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">
              Featured stays
            </h2>
            <p className="mt-1 text-sm text-muted-foreground">
              Hand-picked properties loved by travelers
            </p>
          </div>
          <Button variant="ghost" className="hidden gap-1 sm:inline-flex">
            View all
            <HugeiconsIcon icon={ArrowRight01Icon} size={14} strokeWidth={2} />
          </Button>
        </div>

        <div className="grid gap-4 sm:grid-cols-2 sm:gap-6 lg:grid-cols-3">
          {DEMO_FEATURED_STAYS.map((stay) => (
            <StayCard key={stay.name} stay={stay} />
          ))}
        </div>
      </section>

      <SiteFooter />
    </div>
  )
}
