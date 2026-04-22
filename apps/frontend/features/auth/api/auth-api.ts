import type {
  AuthSession,
  CurrentUser,
  LoginPayload,
  RegisterPayload,
} from "@bookyourstay/shared"
import { apiRequest, ApiClientError, getApiBaseUrl } from "@/lib/api-client"
import { AUTH_ENDPOINTS } from "./auth-endpoints"

export { ApiClientError, getApiBaseUrl }

export function register(payload: RegisterPayload) {
  return apiRequest<AuthSession>(AUTH_ENDPOINTS.register, {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export function login(payload: LoginPayload) {
  return apiRequest<AuthSession>(AUTH_ENDPOINTS.login, {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export function refreshSession() {
  return apiRequest<AuthSession>(AUTH_ENDPOINTS.refresh, {
    method: "POST",
  })
}

export function logout() {
  return apiRequest<null>(AUTH_ENDPOINTS.logout, {
    method: "POST",
  })
}

export function getCurrentUser() {
  return apiRequest<CurrentUser>(AUTH_ENDPOINTS.currentUser, { method: "GET" })
}
