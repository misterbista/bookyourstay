"use client"

import { useSyncExternalStore } from "react"
import { useTheme } from "next-themes"

import { HugeiconsIcon } from "@hugeicons/react"
import { Moon02Icon, Sun02Icon } from "@hugeicons/core-free-icons"

import { Button } from "@/components/ui/button"

type ThemeToggleButtonProps = {
  variant?: "ghost" | "outline"
  size?: "icon" | "icon-sm" | "icon-lg"
  className?: string
}

function ThemeToggleButton({
  variant = "ghost",
  size = "icon",
  className,
}: ThemeToggleButtonProps) {
  const { resolvedTheme, setTheme } = useTheme()
  const mounted = useSyncExternalStore(
    () => () => {},
    () => true,
    () => false,
  )

  const isDark = resolvedTheme === "dark"

  return (
    <Button
      variant={variant}
      size={size}
      className={className}
      aria-label={isDark ? "Switch to light mode" : "Switch to dark mode"}
      onClick={() => setTheme(isDark ? "light" : "dark")}
    >
      <HugeiconsIcon
        icon={mounted && isDark ? Sun02Icon : Moon02Icon}
        size={16}
        strokeWidth={2}
      />
    </Button>
  )
}

export { ThemeToggleButton }
