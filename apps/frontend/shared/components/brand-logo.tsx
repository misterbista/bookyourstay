"use client"

type BrandLogoProps = {
  className?: string
}

function BrandLogo({ className }: BrandLogoProps) {
  return (
    <span
      className={`inline-flex items-baseline font-semibold tracking-tight select-none ${className ?? "text-2xl"}`}
    >
      <span className="text-primary">Book</span>
      <span className="text-foreground font-bold">YourStay</span>
    </span>
  )
}

export { BrandLogo }
