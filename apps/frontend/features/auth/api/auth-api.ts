import type {
  AuthSession,
  CurrentUser,
  LoginPayload,
  RegisterPayload,
} from "../types/auth"
import { apiRequest, ApiClientError, getApiBaseUrl } from "@/lib/api-client"

export { ApiClientError, getApiBaseUrl }

const endpoints = {
  register: "/auth/register",
  login: "/auth/login",
  refresh: "/auth/refresh",
  logout: "/auth/logout",
  currentUser: "/auth/me",
} as const

export function register(payload: RegisterPayload) {
  return apiRequest<AuthSession>(endpoints.register, {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export function login(payload: LoginPayload) {
  return apiRequest<AuthSession>(endpoints.login, {
    method: "POST",
    body: JSON.stringify(payload),
  })
}

export function refreshSession() {
  return apiRequest<AuthSession>(endpoints.refresh, {
    method: "POST",
  })
}

export function logout() {
  return apiRequest<null>(endpoints.logout, {
    method: "POST",
  })
}

export function getCurrentUser() {
  return apiRequest<CurrentUser>(endpoints.currentUser, { method: "GET" })
}
