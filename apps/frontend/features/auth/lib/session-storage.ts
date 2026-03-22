import type { AuthSession } from "@/lib/auth-api"

const storageKey = "bookyourstay.auth.session"

function readStoredSession(): AuthSession | null {
  if (typeof window === "undefined") return null

  const raw = window.localStorage.getItem(storageKey)
  if (!raw) return null

  try {
    return JSON.parse(raw) as AuthSession
  } catch {
    window.localStorage.removeItem(storageKey)
    return null
  }
}

function persistSession(session: AuthSession | null) {
  if (typeof window === "undefined") return

  if (!session) {
    window.localStorage.removeItem(storageKey)
    return
  }

  window.localStorage.setItem(storageKey, JSON.stringify(session))
}

export { persistSession, readStoredSession }
