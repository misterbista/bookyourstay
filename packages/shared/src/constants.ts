export const AUTH_MODES = {
  LOGIN: "login",
  REGISTER: "register",
} as const

export type AuthMode = (typeof AUTH_MODES)[keyof typeof AUTH_MODES]

export const VALIDATION_RULES = {
  PASSWORD_MIN_LENGTH: 8,
  NAME_MIN_LENGTH: 2,
  NAME_MAX_LENGTH: 150,
  EMAIL_MAX_LENGTH: 255,
} as const
