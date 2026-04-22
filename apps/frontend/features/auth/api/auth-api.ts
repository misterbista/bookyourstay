import type {
  ApiFailure,
  ApiSuccess,
  AuthSession,
  CurrentUser,
  LoginPayload,
  RegisterPayload,
} from "@bookyourstay/shared"

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
  init: RequestInit = {}
): Promise<ApiSuccess<T>> {
  let response: Response

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        "Content-Type": "application/json",
        ...(init.headers ?? {}),
      },
      credentials: "include",
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

export function logout() {
  return request<null>("/auth/logout", {
    method: "POST",
  })
}

export function getCurrentUser() {
  return request<CurrentUser>("/auth/me", { method: "GET" })
}
