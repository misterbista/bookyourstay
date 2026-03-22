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

export type AuthSession = {
  userId: string
  fullName: string
  email: string
  accessToken: string
  accessTokenExpiresAt: string
  refreshToken: string
  refreshTokenExpiresAt: string
  sessionId: string
  status: string
  emailVerifiedAt: string | null
}

export type CurrentUser = {
  userId: string
  fullName: string
  email: string
  status: string
  emailVerifiedAt: string | null
  createdAt: string
  lastLoginAt: string | null
}

export type PasswordResetTicket = {
  resetToken: string
  expiresAt: string
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

export type ResetPasswordPayload = {
  resetToken: string
  newPassword: string
}

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "") ??
  "http://localhost:8080/api/v1"

export class ApiClientError extends Error {
  readonly status: number
  readonly errors: Record<string, string[]>

  constructor(
    message: string,
    status: number,
    errors: Record<string, string[]> = {}
  ) {
    super(message)
    this.name = "ApiClientError"
    this.status = status
    this.errors = errors
  }
}

function normalizeErrors(payload: unknown): Record<string, string[]> {
  if (
    typeof payload === "object" &&
    payload !== null &&
    "errors" in payload &&
    typeof payload.errors === "object" &&
    payload.errors !== null
  ) {
    return payload.errors as Record<string, string[]>
  }

  return {}
}

async function parseJson(response: Response) {
  const text = await response.text()
  if (!text) return null

  try {
    return JSON.parse(text) as ApiSuccess<unknown> | ApiFailure
  } catch {
    return null
  }
}

async function request<T>(
  path: string,
  init: RequestInit = {},
  token?: string
): Promise<ApiSuccess<T>> {
  let response: Response

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...(init.headers ?? {}),
      },
      cache: "no-store",
    })
  } catch {
    throw new ApiClientError(
      "Unable to reach the API. Check that the backend is running and accessible from the frontend.",
      0
    )
  }

  const payload = await parseJson(response)

  if (!response.ok) {
    const message =
      payload && "message" in payload && typeof payload.message === "string"
        ? payload.message
        : "Request failed."

    throw new ApiClientError(message, response.status, normalizeErrors(payload))
  }

  if (
    !payload ||
    typeof payload !== "object" ||
    !("success" in payload) ||
    payload.success !== true
  ) {
    throw new ApiClientError("Unexpected API response.", response.status)
  }

  return payload as ApiSuccess<T>
}

export function getApiBaseUrl() {
  return API_BASE_URL
}

export function register(payload: RegisterPayload) {
  return request<AuthSession>("/auth/register", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export function login(payload: LoginPayload) {
  return request<AuthSession>("/auth/login", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export function logout(token: string) {
  return request<null>(
    "/auth/logout",
    {
      method: "POST",
    },
    token
  )
}

export function getCurrentUser(token: string) {
  return request<CurrentUser>("/auth/me", { method: "GET" }, token)
}

export function forgotPassword(email: string) {
  return request<PasswordResetTicket>("/auth/forgot-password", {
    method: "POST",
    body: JSON.stringify({ email }),
  })
}

export function resetPassword(payload: ResetPasswordPayload) {
  return request<null>("/auth/reset-password", {
    method: "POST",
    body: JSON.stringify(payload),
  })
}
