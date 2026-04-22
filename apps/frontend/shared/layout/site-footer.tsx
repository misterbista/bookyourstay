import { Separator } from "@/components/ui/separator"
import { FOOTER_LINK_GROUPS } from "@/shared/config/navigation"

function SiteFooter() {
  return (
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

          {FOOTER_LINK_GROUPS.map((group) => (
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
  )
}

export { SiteFooter }
