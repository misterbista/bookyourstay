"use client"

import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"

import { HugeiconsIcon } from "@hugeicons/react"
import {
  ArrowRight01Icon,
  Calendar03Icon,
  FavouriteIcon,
  Location01Icon,
  Menu01Icon,
  Search01Icon,
  StarIcon,
  UserGroupIcon,
  MapsGlobal01Icon,
} from "@hugeicons/core-free-icons"
import type { DateRange } from "react-day-picker"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Calendar } from "@/components/ui/calendar"
import { Separator } from "@/components/ui/separator"
import { cn } from "@/lib/utils"
import { BrandLogo } from "@/shared/components/brand-logo"
import { ThemeToggleButton } from "@/shared/components/theme-toggle-button"

import {
  CATEGORIES,
  DESTINATIONS,
  FEATURED_STAYS,
  NAV_LINKS,
  POPULAR_SEARCHES,
} from "@/features/home/data/home-content"

function Navbar() {
  const router = useRouter()
  const [scrolled, setScrolled] = useState(false)
  const [mobileOpen, setMobileOpen] = useState(false)

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 20)
    window.addEventListener("scroll", onScroll, { passive: true })
    return () => window.removeEventListener("scroll", onScroll)
  }, [])

  return (
    <header
      className={cn(
        "fixed inset-x-0 top-0 z-50 transition-all duration-300",
        scrolled
          ? "border-b border-border/50 bg-background/80 backdrop-blur-xl"
          : "bg-transparent",
      )}
    >
      <nav className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <button
          onClick={() => router.push("/")}
          className="relative"
          aria-label="BookYourStay home"
        >
          <BrandLogo className="h-10 w-44" sizes="176px" priority />
        </button>

        <div className="hidden items-center gap-1 md:flex">
          {NAV_LINKS.map((link) => (
            <Button
              key={link}
              variant="ghost"
              size="lg"
              className="text-muted-foreground hover:text-foreground"
            >
              {link}
            </Button>
          ))}
        </div>

        <div className="flex items-center gap-2">
          <ThemeToggleButton />
          <Button
            variant="default"
            size="lg"
            className="hidden rounded-xl sm:inline-flex"
            onClick={() => router.push("/auth")}
          >
            Sign in
          </Button>

          <Button
            variant="ghost"
            size="icon"
            className="md:hidden"
            onClick={() => setMobileOpen(!mobileOpen)}
          >
            <HugeiconsIcon icon={Menu01Icon} size={18} strokeWidth={2} />
          </Button>
        </div>
      </nav>

      {mobileOpen && (
        <div className="border-b border-border/50 bg-background/95 px-4 pb-4 backdrop-blur-xl md:hidden">
          <div className="flex flex-col gap-1">
            {NAV_LINKS.map((link) => (
              <Button
                key={link}
                variant="ghost"
                size="lg"
                className="justify-start text-muted-foreground"
              >
                {link}
              </Button>
            ))}
            <Separator className="my-2" />
            <Button
              variant="default"
              size="lg"
              className="rounded-xl"
              onClick={() => router.push("/auth")}
            >
              Sign in
            </Button>
          </div>
        </div>
      )}
    </header>
  )
}

function SearchBar() {
  const [destination, setDestination] = useState("")
  const [dateRange, setDateRange] = useState<DateRange | undefined>()
  const [guests, setGuests] = useState(1)
  const [guestsOpen, setGuestsOpen] = useState(false)

  const checkIn = dateRange?.from
  const checkOut = dateRange?.to

  const formatDate = (date: Date) =>
    date.toLocaleDateString("en-US", { month: "short", day: "numeric" })

  return (
    <div className="w-full max-w-6xl text-left">
      <div className="rounded-2xl border border-border/60 bg-card p-2 shadow-lg dark:shadow-none sm:p-3">
        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:gap-0">
          <div className="group relative flex flex-1 items-center gap-3 rounded-xl px-4 py-3 transition-colors hover:bg-muted/50">
            <HugeiconsIcon
              icon={Location01Icon}
              size={20}
              strokeWidth={1.8}
              className="shrink-0 text-primary"
            />
            <div className="flex flex-1 flex-col">
              <span className="text-[0.65rem] font-semibold uppercase tracking-widest text-muted-foreground">
                Where
              </span>
              <Input
                placeholder="Search destinations..."
                value={destination}
                onChange={(e) => setDestination(e.target.value)}
                className="h-auto border-none bg-transparent p-0 text-sm font-medium shadow-none placeholder:text-muted-foreground/60 focus-visible:ring-0"
              />
            </div>
          </div>

          <Separator orientation="vertical" className="hidden h-10 sm:block" />

          <Popover>
            <PopoverTrigger asChild>
              <button className="flex flex-1 items-center gap-3 rounded-xl px-4 py-3 text-left transition-colors hover:bg-muted/50 sm:max-w-[16rem]">
                <HugeiconsIcon
                  icon={Calendar03Icon}
                  size={20}
                  strokeWidth={1.8}
                  className="shrink-0 text-primary"
                />
                <div className="flex flex-1 flex-col">
                  <span className="text-[0.65rem] font-semibold uppercase tracking-widest text-muted-foreground">
                    When
                  </span>
                  <span className="text-sm font-medium">
                    {checkIn && checkOut
                      ? `${formatDate(checkIn)} - ${formatDate(checkOut)}`
                      : checkIn
                        ? formatDate(checkIn)
                        : "Add dates"}
                  </span>
                </div>
              </button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="range"
                selected={dateRange}
                onSelect={setDateRange}
                numberOfMonths={2}
                disabled={{ before: new Date() }}
                className="rounded-xl"
              />
            </PopoverContent>
          </Popover>

          <Separator orientation="vertical" className="hidden h-10 sm:block" />

          <Popover open={guestsOpen} onOpenChange={setGuestsOpen}>
            <PopoverTrigger asChild>
              <button className="flex items-center gap-3 rounded-xl px-4 py-3 text-left transition-colors hover:bg-muted/50 sm:min-w-[11rem]">
                <HugeiconsIcon
                  icon={UserGroupIcon}
                  size={20}
                  strokeWidth={1.8}
                  className="shrink-0 text-primary"
                />
                <div className="flex flex-col">
                  <span className="text-[0.65rem] font-semibold uppercase tracking-widest text-muted-foreground">
                    Guests
                  </span>
                  <span className="text-sm font-medium">
                    {guests} {guests === 1 ? "guest" : "guests"}
                  </span>
                </div>
              </button>
            </PopoverTrigger>
            <PopoverContent className="w-56" align="end">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Guests</span>
                <div className="flex items-center gap-3">
                  <Button
                    variant="outline"
                    size="icon-sm"
                    disabled={guests <= 1}
                    onClick={() => setGuests(Math.max(1, guests - 1))}
                  >
                    -
                  </Button>
                  <span className="w-6 text-center text-sm font-semibold tabular-nums">
                    {guests}
                  </span>
                  <Button
                    variant="outline"
                    size="icon-sm"
                    disabled={guests >= 16}
                    onClick={() => setGuests(Math.min(16, guests + 1))}
                  >
                    +
                  </Button>
                </div>
              </div>
            </PopoverContent>
          </Popover>

          <Button size="lg" className="shrink-0 rounded-xl px-6 sm:ml-1">
            <HugeiconsIcon icon={Search01Icon} size={16} strokeWidth={2} />
            <span className="sm:inline">Search</span>
          </Button>
        </div>
      </div>
    </div>
  )
}

function DestinationCard({
  destination,
}: {
  destination: (typeof DESTINATIONS)[number]
}) {
  return (
    <div className="group relative overflow-hidden rounded-2xl">
      <div className="aspect-[3/4] w-full overflow-hidden">
        <img
          src={destination.image}
          alt={destination.name}
          className="size-full object-cover transition-transform duration-500 group-hover:scale-110"
        />
      </div>
      <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-black/20 to-transparent" />
      <div className="absolute inset-x-0 bottom-0 p-5">
        <p className="text-xs font-medium uppercase tracking-wider text-white/70">
          {destination.tagline}
        </p>
        <h3 className="mt-1 text-lg font-semibold text-white">
          {destination.name}
        </h3>
        <p className="mt-1 text-sm text-white/80">
          {destination.properties.toLocaleString()} properties
        </p>
      </div>
    </div>
  )
}

function StayCard({ stay }: { stay: (typeof FEATURED_STAYS)[number] }) {
  const [liked, setLiked] = useState(false)

  return (
    <div className="group relative overflow-hidden rounded-2xl">
      <div className="relative aspect-[3/4] w-full overflow-hidden">
        <img
          src={stay.image}
          alt={stay.name}
          className="size-full object-cover transition-transform duration-500 group-hover:scale-110"
        />

        <button
          onClick={() => setLiked(!liked)}
          className={cn(
            "absolute right-3 top-3 z-10 flex size-8 items-center justify-center rounded-full bg-white/80 backdrop-blur-sm transition-colors hover:bg-white",
            liked && "bg-primary/10 text-primary hover:bg-primary/20",
          )}
        >
          <HugeiconsIcon
            icon={FavouriteIcon}
            size={16}
            strokeWidth={liked ? 2.5 : 1.8}
          />
        </button>

        {stay.featured && (
          <Badge className="absolute left-3 top-3 z-10" variant="default">
            Featured
          </Badge>
        )}
        <div className="absolute inset-0 bg-gradient-to-t from-black/88 via-black/40 via-38% to-transparent" />
        <div className="absolute inset-x-0 bottom-0 z-10 p-5">
          <div className="flex items-center justify-between gap-3">
            <p className="text-xs font-medium uppercase tracking-wider text-white/70">
              {stay.type} - {stay.reviews} reviews
            </p>
            <div className="flex items-center gap-1 rounded-full bg-white/14 px-2.5 py-1 text-xs font-semibold text-white backdrop-blur-sm">
              <HugeiconsIcon icon={StarIcon} size={12} strokeWidth={2} />
              {stay.rating}
            </div>
          </div>
          <h3 className="mt-2 text-lg font-semibold text-white">{stay.name}</h3>
          <p className="mt-1 flex items-center gap-1 text-sm text-white/80">
            <HugeiconsIcon
              icon={Location01Icon}
              size={12}
              strokeWidth={2}
            />
            {stay.location}
          </p>
          <p className="mt-3 text-sm font-semibold text-white">
            ${stay.price}
            <span className="ml-1 font-normal text-white/70">/night</span>
          </p>
        </div>
      </div>
    </div>
  )
}

function HomePage() {
  const [activeCategory, setActiveCategory] = useState("All")

  return (
    <div className="min-h-svh bg-background text-foreground">
      <Navbar />

      <section className="relative flex min-h-[85vh] flex-col items-center justify-center overflow-hidden px-4 pt-16">
        <div className="pointer-events-none absolute inset-0 overflow-hidden">
          <div className="absolute -right-32 -top-32 size-96 rounded-full bg-primary/5 blur-3xl" />
          <div className="absolute -bottom-32 -left-32 size-96 rounded-full bg-chart-1/5 blur-3xl" />
          <div className="absolute left-1/2 top-1/3 size-64 -translate-x-1/2 rounded-full bg-chart-2/5 blur-3xl" />
        </div>

        <div className="relative z-10 mx-auto flex w-full max-w-6xl flex-col items-center text-center">
          <Badge variant="secondary" className="mb-6">
            <HugeiconsIcon
              icon={MapsGlobal01Icon}
              size={12}
              strokeWidth={2}
            />
            Over 10,000+ stays worldwide
          </Badge>

          <h1 className="text-4xl font-bold leading-tight tracking-tight sm:text-5xl md:text-6xl">
            Find your perfect{" "}
            <span className="text-primary">getaway</span>
          </h1>

          <p className="mt-4 max-w-xl text-base text-muted-foreground sm:text-lg">
            Discover unique stays, boutique hotels, and curated experiences
            around the world - all in one place.
          </p>

          <div className="mt-10 flex w-full justify-center">
            <SearchBar />
          </div>

          <div className="mt-6 flex flex-wrap items-center justify-center gap-2">
            <span className="text-xs text-muted-foreground">Popular:</span>
            {POPULAR_SEARCHES.map((place) => (
              <Button
                key={place}
                variant="outline"
                size="sm"
                className="rounded-full text-xs"
              >
                {place}
              </Button>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <div className="flex items-center gap-2 overflow-x-auto pb-2">
          {CATEGORIES.map((cat) => (
            <button
              key={cat.label}
              onClick={() => setActiveCategory(cat.label)}
              className={cn(
                "flex shrink-0 items-center gap-2 rounded-full border px-4 py-2 text-sm font-medium transition-all",
                activeCategory === cat.label
                  ? "border-primary bg-primary/10 text-primary"
                  : "border-border bg-card text-muted-foreground hover:border-primary/30 hover:text-foreground",
              )}
            >
              <HugeiconsIcon icon={cat.icon} size={16} strokeWidth={1.8} />
              {cat.label}
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
          {DESTINATIONS.map((dest) => (
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
          {FEATURED_STAYS.map((stay) => (
            <StayCard key={stay.name} stay={stay} />
          ))}
        </div>
      </section>

      <footer className="border-t border-border/50 bg-muted/30">
        <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
          <div className="grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
            <div>
              <div className="flex items-center gap-2 text-lg font-bold">
                <span className="flex size-7 items-center justify-center rounded-lg bg-primary text-sm text-primary-foreground">
                  B
                </span>
                BookYourStay
              </div>
              <p className="mt-3 text-sm leading-relaxed text-muted-foreground">
                Discover and book unique accommodations around the world.
              </p>
            </div>
            {(
              [
                {
                  title: "Company",
                  links: ["About", "Careers", "Press", "Blog"],
                },
                {
                  title: "Support",
                  links: ["Help Center", "Safety", "Cancellation", "Contact"],
                },
                {
                  title: "Legal",
                  links: ["Terms", "Privacy", "Cookies", "Licenses"],
                },
              ] as const
            ).map((group) => (
              <div key={group.title}>
                <h4 className="mb-3 text-sm font-semibold">{group.title}</h4>
                <ul className="space-y-2">
                  {group.links.map((link) => (
                    <li key={link}>
                      <button className="text-sm text-muted-foreground transition-colors hover:text-foreground">
                        {link}
                      </button>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </div>
          <Separator className="my-8" />
          <p className="text-center text-xs text-muted-foreground">
            &copy; {new Date().getFullYear()} BookYourStay. All rights reserved.
          </p>
        </div>
      </footer>
    </div>
  )
}

export { HomePage }
