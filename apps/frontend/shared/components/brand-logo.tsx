"use client"

import Image from "next/image"

type BrandLogoProps = {
  className?: string
  sizes?: string
  priority?: boolean
}

function BrandLogo({
  className = "h-12 w-48",
  sizes = "192px",
  priority = false,
}: BrandLogoProps) {
  return (
    <div className={`relative ${className}`}>
      <Image
        src="/LIGHTMODEBYSLOGO.png"
        alt="BookYourStay"
        fill
        priority={priority}
        sizes={sizes}
        className="object-contain object-left dark:hidden"
      />
      <Image
        src="/DARKMODEBYSLOGO.png"
        alt="BookYourStay"
        fill
        priority={priority}
        sizes={sizes}
        className="hidden object-contain object-left dark:block"
      />
    </div>
  )
}

export { BrandLogo }
