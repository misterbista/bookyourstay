"use client"

import { useState } from "react"
import Image from "next/image"
import { HugeiconsIcon } from "@hugeicons/react"
import {
  FavouriteIcon,
  Location01Icon,
  StarIcon,
} from "@hugeicons/core-free-icons"

import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import type { Stay } from "../types/home"

interface StayCardProps {
  stay: Stay
}

export function StayCard({ stay }: StayCardProps) {
  const [liked, setLiked] = useState(false)

  return (
    <div className="group relative overflow-hidden rounded-2xl">
      <div className="relative aspect-[3/4] w-full overflow-hidden">
        <Image
          src={stay.image}
          alt={stay.name}
          fill
          className="object-cover transition-transform duration-500 group-hover:scale-110"
        />

        <button
          onClick={() => setLiked(!liked)}
          className={cn(
            "absolute top-3 right-3 z-10 flex size-8 items-center justify-center rounded-full bg-white/80 backdrop-blur-sm transition-colors hover:bg-white",
            liked && "bg-primary/10 text-primary hover:bg-primary/20"
          )}
        >
          <HugeiconsIcon
            icon={FavouriteIcon}
            size={16}
            strokeWidth={liked ? 2.5 : 1.8}
          />
        </button>

        {stay.featured && (
          <Badge className="absolute top-3 left-3 z-10" variant="default">
            Featured
          </Badge>
        )}
        <div className="absolute inset-0 bg-gradient-to-t from-black/88 via-black/40 via-38% to-transparent" />
        <div className="absolute inset-x-0 bottom-0 z-10 p-5">
          <div className="flex items-center justify-between gap-3">
            <p className="text-xs font-medium tracking-wider text-white/70 uppercase">
              {stay.type} - {stay.reviews} reviews
            </p>
            <div className="flex items-center gap-1 rounded-full bg-white/14 px-2.5 py-1 text-xs font-semibold text-white backdrop-blur-sm">
              <HugeiconsIcon icon={StarIcon} size={12} strokeWidth={2} />
              {stay.rating}
            </div>
          </div>
          <h3 className="mt-2 text-lg font-semibold text-white">{stay.name}</h3>
          <p className="mt-1 flex items-center gap-1 text-sm text-white/80">
            <HugeiconsIcon icon={Location01Icon} size={12} strokeWidth={2} />
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
