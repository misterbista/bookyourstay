import Link from "next/link"
import { Separator } from "@/components/ui/separator"
import { BrandLogo } from "@/shared/components/brand-logo"

const FOOTER_LINKS = [
  { name: "Destinations", href: "/#destinations" },
  { name: "Featured stays", href: "/#stays" },
  { name: "Sign in", href: "/auth" },
] as const

function SiteFooter() {
  return (
    <footer className="border-t border-border/50 bg-muted/30">
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="flex flex-col gap-8 md:flex-row md:items-start md:justify-between">
          <div className="max-w-sm">
            <BrandLogo className="text-xl" />
            <p className="mt-3 text-sm leading-relaxed text-muted-foreground">
              Discover and book unique accommodations around the world.
            </p>
          </div>

          <nav aria-label="Footer navigation">
            <ul className="flex flex-wrap gap-x-5 gap-y-3">
              {FOOTER_LINKS.map((link) => (
                <li key={link.name}>
                  <Link
                    href={link.href}
                    className="text-sm text-muted-foreground transition-colors hover:text-foreground"
                  >
                    {link.name}
                  </Link>
                </li>
              ))}
            </ul>
          </nav>
        </div>
        <Separator className="my-8" />
        <p className="text-center text-xs text-muted-foreground">
          &copy; {new Date().getFullYear()} BookYourStay. All rights reserved.
        </p>
      </div>
    </footer>
  )
}

export { SiteFooter }
