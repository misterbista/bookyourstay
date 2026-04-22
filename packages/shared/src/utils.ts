export function flattenErrors(error: {
  message: string
  errors?: Record<string, string[]>
}) {
  const details = Object.values(error.errors ?? {}).flat().filter(Boolean)
  return details.length > 0 ? details.join(" ") : error.message
}

export function formatDate(value: string | Date | null | undefined): string {
  if (!value) return "Not available"

  const date = typeof value === "string" ? new Date(value) : value

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date)
}
