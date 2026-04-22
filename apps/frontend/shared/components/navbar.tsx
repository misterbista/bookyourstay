"use client"

import { useEffect, useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"

import { HugeiconsIcon } from "@hugeicons/react"
import { Menu01Icon } from "@hugeicons/core-free-icons"

import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { cn } from "@/lib/utils"
import { BrandLogo } from "@/shared/components/brand-logo"
import { ThemeToggleButton } from "@/shared/components/theme-toggle-button"

import { NAV_LINKS } from "@/features/home/data/home-content"

type NavbarProps = {
  variant?: "home" | "auth"
}

function Navbar({ variant = "home" }: NavbarProps) {
  const router = useRouter()
  const [scrolled, setScrolled] = useState(false)
  const [mobileOpen, setMobileOpen] = useState(false)

  const isHome = variant === "home"

  useEffect(() => {
    if (!isHome) return
    const onScroll = () => setScrolled(window.scrollY > 20)
    window.addEventListener("scroll", onScroll, { passive: true })
    return () => window.removeEventListener("scroll", onScroll)
  }, [isHome])

  if (!isHome) {
    return (
      <header className="flex items-center justify-between px-6 py-4 sm:px-10">
        <Link href="/" aria-label="BookYourStay home">
          <BrandLogo className="text-xl sm:text-2xl" />
        </Link>
        <ThemeToggleButton
          variant="outline"
          className="rounded-xl border-border/60 bg-background/80 text-muted-foreground hover:bg-muted hover:text-foreground"
        />
      </header>
    )
  }

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
          <BrandLogo className="text-2xl" />
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

export { Navbar }
