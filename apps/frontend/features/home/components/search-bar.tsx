"use client"

import { useState } from "react"
import { HugeiconsIcon } from "@hugeicons/react"
import {
  Calendar03Icon,
  Location01Icon,
  Search01Icon,
  UserGroupIcon,
} from "@hugeicons/core-free-icons"
import type { DateRange } from "react-day-picker"

import { Button } from "@/components/ui/button"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { Calendar } from "@/components/ui/calendar"
import type { SearchCriteria } from "../types/home"

interface SearchBarProps {
  onSearch: (searchData: SearchCriteria) => void
}

export function SearchBar({ onSearch }: SearchBarProps) {
  const [destination, setDestination] = useState("")
  const [dateRange, setDateRange] = useState<DateRange | undefined>()
  const [guests, setGuests] = useState(1)
  const [guestsOpen, setGuestsOpen] = useState(false)

  const checkIn = dateRange?.from
  const checkOut = dateRange?.to

  const formatDate = (date: Date) =>
    date.toLocaleDateString("en-US", { month: "short", day: "numeric" })

  const handleSearch = () => {
    onSearch({
      destination,
      checkIn,
      checkOut,
      guests,
    })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    handleSearch()
  }

  return (
    <form onSubmit={handleSubmit} className="w-full max-w-6xl text-left">
      <div className="rounded-[1.4rem] border border-border/60 bg-card p-2 shadow-lg sm:px-3 sm:py-2.5 dark:shadow-none">
        <div className="flex flex-col gap-2 sm:grid sm:grid-cols-[minmax(0,1.4fr)_1px_minmax(0,0.95fr)_1px_minmax(0,0.8fr)_auto] sm:items-center sm:gap-0">
          <div className="group relative px-1.5 py-1 sm:px-2">
            <div className="flex min-h-16 items-center gap-3 rounded-xl px-4 py-3 transition-colors focus-within:bg-muted/50 hover:bg-muted/50">
              <HugeiconsIcon
                icon={Location01Icon}
                size={20}
                strokeWidth={1.8}
                className="shrink-0 text-primary"
              />
              <div className="flex flex-1 flex-col">
                <span className="text-[0.65rem] font-semibold tracking-widest text-muted-foreground uppercase">
                  Where
                </span>
                <input
                  aria-label="Destination"
                  type="text"
                  placeholder="Search destinations..."
                  value={destination}
                  onChange={(e) => setDestination(e.target.value)}
                  className="h-6 w-full border-0 bg-transparent p-0 text-[15px] leading-6 font-medium text-foreground outline-none placeholder:text-muted-foreground/60"
                />
              </div>
            </div>
          </div>

          <div
            className="hidden h-10 w-px bg-border sm:block"
            aria-hidden="true"
          />

          <Popover>
            <PopoverTrigger asChild>
              <button
                type="button"
                className="w-full px-1.5 py-1 text-left sm:px-2"
              >
                <span className="flex min-h-16 items-center gap-3 rounded-xl px-4 py-3 transition-colors hover:bg-muted/50">
                  <HugeiconsIcon
                    icon={Calendar03Icon}
                    size={20}
                    strokeWidth={1.8}
                    className="shrink-0 text-primary"
                  />
                  <span className="flex flex-1 flex-col">
                    <span className="text-[0.65rem] font-semibold tracking-widest text-muted-foreground uppercase">
                      When
                    </span>
                    <span className="text-sm font-medium">
                      {checkIn && checkOut
                        ? `${formatDate(checkIn)} - ${formatDate(checkOut)}`
                        : checkIn
                          ? formatDate(checkIn)
                          : "Add dates"}
                    </span>
                  </span>
                </span>
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

          <div
            className="hidden h-10 w-px bg-border sm:block"
            aria-hidden="true"
          />

          <Popover open={guestsOpen} onOpenChange={setGuestsOpen}>
            <PopoverTrigger asChild>
              <button
                type="button"
                className="w-full px-1.5 py-1 text-left sm:px-2"
              >
                <span className="flex min-h-16 items-center gap-3 rounded-xl px-4 py-3 transition-colors hover:bg-muted/50">
                  <HugeiconsIcon
                    icon={UserGroupIcon}
                    size={20}
                    strokeWidth={1.8}
                    className="shrink-0 text-primary"
                  />
                  <span className="flex flex-col">
                    <span className="text-[0.65rem] font-semibold tracking-widest text-muted-foreground uppercase">
                      Guests
                    </span>
                    <span className="text-sm font-medium">
                      {guests} {guests === 1 ? "guest" : "guests"}
                    </span>
                  </span>
                </span>
              </button>
            </PopoverTrigger>
            <PopoverContent className="w-56" align="end">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Guests</span>
                <div className="flex items-center gap-3">
                  <Button
                    type="button"
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
                    type="button"
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

          <Button
            type="submit"
            size="lg"
            className="min-h-12 rounded-xl px-7 sm:ml-2 sm:min-w-36 sm:self-center"
          >
            <HugeiconsIcon icon={Search01Icon} size={16} strokeWidth={2} />
            <span className="sm:inline">Search</span>
          </Button>
        </div>
      </div>
    </form>
  )
}
