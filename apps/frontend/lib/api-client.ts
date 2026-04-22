const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "") ??
  "http://localhost:8080/api/v1"

export type ApiSuccess<T> = {
  success: true
  message: string
  data: T
}

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

export function flattenErrors(error: {
  message: string
  errors?: Record<string, string[]>
}) {
  const details = Object.values(error.errors ?? {})
    .flat()
    .filter(Boolean)
  return details.length > 0 ? details.join(" ") : error.message
}

export async function apiRequest<T>(
  path: string,
  init: RequestInit = {}
): Promise<ApiSuccess<T>> {
  let response: Response
  const headers = new Headers(init.headers)

  if (init.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json")
  }

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers,
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
      isRecord(payload) && typeof payload.message === "string"
        ? payload.message
        : "Request failed."

    throw new ApiClientError(message, response.status, normalizeErrors(payload))
  }

  if (!isApiSuccess<T>(payload)) {
    throw new ApiClientError("Unexpected API response.", response.status)
  }

  return payload
}

function normalizeErrors(payload: unknown): Record<string, string[]> {
  if (!isRecord(payload) || !isRecord(payload.errors)) {
    return {}
  }

  return Object.fromEntries(
    Object.entries(payload.errors).filter(
      (entry): entry is [string, string[]] =>
        Array.isArray(entry[1]) &&
        entry[1].every((value) => typeof value === "string")
    )
  )
}

async function parseJson(response: Response) {
  const text = await response.text()
  if (!text) return null

  try {
    return JSON.parse(text) as unknown
  } catch {
    return null
  }
}

function isApiSuccess<T>(value: unknown): value is ApiSuccess<T> {
  return (
    isRecord(value) &&
    value.success === true &&
    typeof value.message === "string" &&
    "data" in value
  )
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null
}
