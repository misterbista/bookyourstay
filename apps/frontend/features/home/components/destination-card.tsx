import Image from "next/image"
import type { Destination } from "../types/home"

interface DestinationCardProps {
  destination: Destination
}

export function DestinationCard({ destination }: DestinationCardProps) {
  return (
    <div className="group relative overflow-hidden rounded-lg">
      <div className="relative aspect-[3/4] w-full overflow-hidden">
        <Image
          src={destination.image}
          alt={destination.name}
          fill
          sizes="(min-width: 1024px) 25vw, 50vw"
          className="object-cover transition-transform duration-500 group-hover:scale-110"
        />
      </div>
      <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-black/20 to-transparent" />
      <div className="absolute inset-x-0 bottom-0 p-5">
        <p className="text-xs font-medium tracking-wider text-white/70 uppercase">
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
