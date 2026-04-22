export const NAV_LINKS = [
  { name: "Home", href: "/" },
  { name: "Destinations", href: "/destinations" },
  { name: "About", href: "/about" },
  { name: "Contact", href: "/contact" },
] as const

export const FOOTER_LINK_GROUPS = [
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
