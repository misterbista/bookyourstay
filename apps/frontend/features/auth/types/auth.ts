export type AuthMode = "login" | "register"

export type AuthSession = {
  sessionId: string
}

export type CurrentUser = {
  id: string
  fullName: string
  email: string
  status: string
  emailVerifiedAt: string | null
  createdAt: string
  lastLoginAt: string | null
}

export type RegisterPayload = {
  fullName: string
  email: string
  password: string
}

export type LoginPayload = {
  email: string
  password: string
}

export type FeedbackState = {
  tone: "success" | "error" | "info" | "warning"
  title: string
  body?: string
}
