export type ApiSuccess<T> = {
  success: true
  message: string
  data: T
  meta?: unknown
}

export type ApiFailure = {
  success: false
  message: string
  errors?: Record<string, string[]>
}

export type ApiResponse<T> = ApiSuccess<T> | ApiFailure

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
