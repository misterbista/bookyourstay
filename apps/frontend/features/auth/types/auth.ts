import type {
  ApiFailure,
  ApiResponse,
  AuthMode,
  AuthSession,
  CurrentUser,
  LoginPayload,
  RegisterPayload,
} from "@bookyourstay/shared"

export type {
  ApiFailure,
  ApiResponse,
  AuthMode,
  AuthSession,
  CurrentUser,
  LoginPayload,
  RegisterPayload,
}

export type FeedbackState = {
  tone: "success" | "error" | "info" | "warning"
  title: string
  body?: string
}
