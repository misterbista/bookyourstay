"use client"

import { type FormEvent, useEffect, useState } from "react"

import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
  ApiClientError,
  type AuthSession,
  type CurrentUser,
  type PasswordResetTicket,
  forgotPassword,
  getApiBaseUrl,
  getCurrentUser,
  login,
  logout,
  register,
  resetPassword,
} from "@/lib/auth-api"
import { cn } from "@/lib/utils"

const storageKey = "bookyourstay.auth.session"

type AuthMode = "login" | "register" | "recovery"

type FeedbackState = {
  tone: "success" | "error" | "info"
  title: string
  body: string
} | null

const authModes: Array<{ id: AuthMode; label: string; hint: string }> = [
  { id: "login", label: "Sign in", hint: "Resume a stay plan in seconds." },
  { id: "register", label: "Create account", hint: "Start with a traveler profile." },
  { id: "recovery", label: "Reset access", hint: "Issue and redeem a reset token." },
]

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

function formatDate(value: string | null) {
  if (!value) return "Not available"

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value))
}

function flattenErrors(error: ApiClientError) {
  const details = Object.values(error.errors)
    .flat()
    .filter(Boolean)

  return details.length > 0 ? details.join(" ") : error.message
}

function Field({
  label,
  hint,
  children,
}: {
  label: string
  hint?: string
  children: React.ReactNode
}) {
  return (
    <label className="flex flex-col gap-2">
      <span className="flex items-center justify-between gap-4 text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
        <span>{label}</span>
        {hint ? <span className="tracking-normal lowercase">{hint}</span> : null}
      </span>
      {children}
    </label>
  )
}

function SchemaFact({
  label,
  value,
}: {
  label: string
  value: string
}) {
  return (
    <div className="rounded-2xl border border-border/60 bg-background/75 p-4">
      <div className="text-[0.68rem] font-semibold tracking-[0.24em] text-muted-foreground uppercase">
        {label}
      </div>
      <div className="mt-2 font-mono text-xs leading-6 text-foreground">{value}</div>
    </div>
  )
}

function Panel({
  title,
  description,
  children,
}: {
  title: string
  description: string
  children: React.ReactNode
}) {
  return (
    <Card className="overflow-hidden border-black/5 bg-white/85 shadow-[0_20px_70px_rgba(90,54,22,0.12)] backdrop-blur">
      <CardHeader className="pb-5">
        <CardTitle>{title}</CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent>{children}</CardContent>
    </Card>
  )
}

function statusVariant(status: string) {
  if (status === "active") return "success" as const
  if (status === "pending") return "warning" as const
  return "outline" as const
}

export function AuthWorkspace() {
  const [mode, setMode] = useState<AuthMode>("login")
  const [session, setSession] = useState<AuthSession | null>(null)
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null)
  const [isHydratingSession, setIsHydratingSession] = useState(true)
  const [busyAction, setBusyAction] = useState<string | null>(null)
  const [feedback, setFeedback] = useState<FeedbackState>(null)
  const [resetTicket, setResetTicket] = useState<PasswordResetTicket | null>(null)

  const [registerForm, setRegisterForm] = useState({
    fullName: "",
    email: "",
    password: "",
  })
  const [loginForm, setLoginForm] = useState({
    email: "",
    password: "",
  })
  const [forgotEmail, setForgotEmail] = useState("")
  const [resetForm, setResetForm] = useState({
    resetToken: "",
    newPassword: "",
  })

  useEffect(() => {
    async function hydrate() {
      const stored = readStoredSession()
      if (!stored) {
        setIsHydratingSession(false)
        return
      }

      setSession(stored)

      try {
        const response = await getCurrentUser(stored.accessToken)
        setCurrentUser(response.data)
      } catch {
        persistSession(null)
        setSession(null)
        setCurrentUser(null)
      } finally {
        setIsHydratingSession(false)
      }
    }

    void hydrate()
  }, [])

  async function refreshCurrentUser(activeSession: AuthSession) {
    const response = await getCurrentUser(activeSession.accessToken)
    setCurrentUser(response.data)
  }

  async function handleRegister(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusyAction("register")
    setFeedback(null)

    try {
      const response = await register(registerForm)
      persistSession(response.data)
      setSession(response.data)
      await refreshCurrentUser(response.data)
      setMode("login")
      setRegisterForm({ fullName: "", email: "", password: "" })
      setFeedback({
        tone: "success",
        title: "Account created",
        body: "The frontend is now wired to the live backend registration flow.",
      })
    } catch (error) {
      const message =
        error instanceof ApiClientError
          ? flattenErrors(error)
          : "Registration failed."

      setFeedback({
        tone: "error",
        title: "Registration failed",
        body: message,
      })
    } finally {
      setBusyAction(null)
      setIsHydratingSession(false)
    }
  }

  async function handleLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusyAction("login")
    setFeedback(null)

    try {
      const response = await login(loginForm)
      persistSession(response.data)
      setSession(response.data)
      await refreshCurrentUser(response.data)
      setLoginForm({ email: "", password: "" })
      setFeedback({
        tone: "success",
        title: "Signed in",
        body: "Your access token is stored locally and used for the `/auth/me` check.",
      })
    } catch (error) {
      const message =
        error instanceof ApiClientError ? flattenErrors(error) : "Login failed."

      setFeedback({
        tone: "error",
        title: "Unable to sign in",
        body: message,
      })
    } finally {
      setBusyAction(null)
      setIsHydratingSession(false)
    }
  }

  async function handleForgotPassword(event: { preventDefault: () => void }) {
    event.preventDefault()
    setBusyAction("forgot")
    setFeedback(null)

    try {
      const response = await forgotPassword(forgotEmail)
      setResetTicket(response.data)
      setResetForm((current) => ({
        ...current,
        resetToken: response.data.resetToken,
      }))
      setFeedback({
        tone: "success",
        title: "Reset token issued",
        body: "This backend currently returns the token directly, so you can test the full reset flow from the UI.",
      })
    } catch (error) {
      const message =
        error instanceof ApiClientError
          ? flattenErrors(error)
          : "Password reset could not be requested."

      setFeedback({
        tone: "error",
        title: "Reset request failed",
        body: message,
      })
    } finally {
      setBusyAction(null)
    }
  }

  async function handleResetPassword(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusyAction("reset")
    setFeedback(null)

    try {
      await resetPassword(resetForm)
      setResetForm({ resetToken: "", newPassword: "" })
      setFeedback({
        tone: "success",
        title: "Password updated",
        body: "Use the new password in the sign-in form to confirm the flow end to end.",
      })
      setMode("login")
    } catch (error) {
      const message =
        error instanceof ApiClientError
          ? flattenErrors(error)
          : "Password reset failed."

      setFeedback({
        tone: "error",
        title: "Password update failed",
        body: message,
      })
    } finally {
      setBusyAction(null)
    }
  }

  async function handleLogout() {
    if (!session) return

    setBusyAction("logout")
    setFeedback(null)

    try {
      await logout(session.accessToken)
      setFeedback({
        tone: "info",
        title: "Signed out",
        body: "The local session was cleared and the backend logout endpoint was called.",
      })
    } catch {
      setFeedback({
        tone: "info",
        title: "Local session cleared",
        body: "The frontend signed out locally even though the logout request could not be confirmed.",
      })
    } finally {
      persistSession(null)
      setSession(null)
      setCurrentUser(null)
      setBusyAction(null)
    }
  }

  async function handleRefreshProfile() {
    if (!session) return

    setBusyAction("profile")
    setFeedback(null)

    try {
      await refreshCurrentUser(session)
      setFeedback({
        tone: "success",
        title: "Profile refreshed",
        body: "The current user card was reloaded from `/api/v1/auth/me`.",
      })
    } catch (error) {
      const message =
        error instanceof ApiClientError
          ? flattenErrors(error)
          : "Unable to refresh the current user."

      setFeedback({
        tone: "error",
        title: "Profile refresh failed",
        body: message,
      })
    } finally {
      setBusyAction(null)
    }
  }

  return (
    <main className="min-h-screen overflow-hidden bg-[radial-gradient(circle_at_top_left,rgba(184,97,30,0.18),transparent_30%),linear-gradient(180deg,#f6efe5_0%,#fffaf4_46%,#efe5d8_100%)] text-foreground">
      <div className="mx-auto grid min-h-screen max-w-7xl gap-8 px-5 py-6 lg:grid-cols-[1.08fr_0.92fr] lg:px-8">
        <section className="relative overflow-hidden rounded-[2.5rem] border border-stone-900/10 bg-[linear-gradient(150deg,rgba(36,24,15,0.98),rgba(95,58,26,0.94)_40%,rgba(188,107,43,0.88)_100%)] p-8 text-white shadow-[0_35px_120px_rgba(71,38,12,0.28)] sm:p-10 lg:p-12">
          <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_right,rgba(255,255,255,0.2),transparent_26%),radial-gradient(circle_at_bottom_left,rgba(255,229,188,0.2),transparent_30%)]" />

          <div className="relative flex h-full flex-col gap-8">
            <div className="space-y-5">
              <Badge variant="outline" className="border-white/20 bg-white/8 text-white">
                Traveler IAM Flow
              </Badge>
              <div className="max-w-2xl space-y-4">
                <h1 className="max-w-xl text-4xl font-semibold tracking-tight sm:text-5xl">
                  Local identity, real session records, and reset tokens tied to your schema.
                </h1>
                <p className="max-w-xl text-base leading-7 text-white/78">
                  This screen now matches the live auth slice: a local email/password identity in <span className="font-mono text-white">iam.user_identities</span>, a traveler record in <span className="font-mono text-white">iam.users</span>, a hashed refresh session in <span className="font-mono text-white">iam.user_sessions</span>, and reset tickets in <span className="font-mono text-white">iam.password_reset_tokens</span>.
                </p>
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <SchemaFact label="Current API" value="POST /api/v1/auth/register | login | forgot-password | reset-password" />
              <SchemaFact label="Identity Provider" value="local provider_subject = normalized email" />
              <SchemaFact label="Session Model" value="JWT access token + hashed refresh token + session public_id" />
              <SchemaFact label="Schema Gap" value="Phone, device_name, ip_address, partner roles exist in schema but are not collected by these endpoints yet" />
            </div>

            <div className="grid gap-4 md:grid-cols-3">
              {[
                {
                  title: "Register",
                  body: "Creates iam.users, inserts a local identity row, and opens a session immediately.",
                },
                {
                  title: "Login",
                  body: "Validates the local identity, updates last_login_at, and creates a new session record.",
                },
                {
                  title: "Recovery",
                  body: "Issues a reset token, then consumes it to rotate the local password hash.",
                },
              ].map((item) => (
                <div
                  key={item.title}
                  className="rounded-[1.65rem] border border-white/15 bg-white/8 p-4 backdrop-blur"
                >
                  <div className="text-sm font-semibold">{item.title}</div>
                  <p className="mt-2 text-sm leading-6 text-white/72">{item.body}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="flex flex-col gap-5 pb-6 lg:pb-0">
          <div className="rounded-[2rem] border border-black/5 bg-white/75 p-3 shadow-[0_18px_60px_rgba(102,62,22,0.08)] backdrop-blur">
            <div className="grid grid-cols-3 gap-2">
              {authModes.map((authMode) => (
                <button
                  key={authMode.id}
                  type="button"
                  onClick={() => setMode(authMode.id)}
                  className={cn(
                    "rounded-[1.35rem] px-3 py-3 text-left transition",
                    mode === authMode.id
                      ? "bg-foreground text-background shadow-sm"
                      : "bg-transparent text-muted-foreground hover:bg-background/70 hover:text-foreground"
                  )}
                >
                  <div className="text-sm font-semibold">{authMode.label}</div>
                  <div className="mt-1 text-xs leading-5 opacity-80">
                    {authMode.hint}
                  </div>
                </button>
              ))}
            </div>
          </div>

          {feedback ? (
            <div
              className={cn(
                "rounded-[1.75rem] border px-5 py-4 text-sm shadow-sm animate-in fade-in slide-in-from-top-3 duration-300",
                feedback.tone === "success" &&
                  "border-emerald-200 bg-emerald-50 text-emerald-950",
                feedback.tone === "error" &&
                  "border-rose-200 bg-rose-50 text-rose-950",
                feedback.tone === "info" &&
                  "border-sky-200 bg-sky-50 text-sky-950"
              )}
            >
              <div className="font-semibold">{feedback.title}</div>
              <p className="mt-1 leading-6 opacity-85">{feedback.body}</p>
            </div>
          ) : null}

          {isHydratingSession ? (
            <Panel
              title="Checking saved session"
              description="Rehydrating the saved access token and validating it against the live /auth/me endpoint."
            >
              <div className="h-28 animate-pulse rounded-[1.5rem] bg-muted/70" />
            </Panel>
          ) : session && currentUser ? (
            <Panel
              title={`Welcome back, ${currentUser.fullName}`}
              description="This summary comes from the live current-user query using the stored bearer token and session claims."
            >
              <div className="grid gap-4">
                <div className="grid gap-3 rounded-[1.5rem] border border-border/70 bg-muted/35 p-4">
                  <div className="flex flex-wrap items-center gap-2">
                    <Badge variant={statusVariant(currentUser.status)}>
                      User status: {currentUser.status}
                    </Badge>
                    <Badge
                      variant={
                        currentUser.emailVerifiedAt ? "success" : "warning"
                      }
                    >
                      {currentUser.emailVerifiedAt
                        ? "Email verified"
                        : "Email not verified"}
                    </Badge>
                  </div>
                  <div className="grid gap-3 sm:grid-cols-2">
                    <div>
                      <div className="text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
                        Email
                      </div>
                      <div className="mt-1 text-sm font-medium">{currentUser.email}</div>
                    </div>
                    <div>
                      <div className="text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
                        User public ID
                      </div>
                      <div className="mt-1 break-all font-mono text-xs leading-6">
                        {currentUser.userId}
                      </div>
                    </div>
                    <div>
                      <div className="text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
                        Created
                      </div>
                      <div className="mt-1 text-sm font-medium">
                        {formatDate(currentUser.createdAt)}
                      </div>
                    </div>
                    <div>
                      <div className="text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
                        Last login
                      </div>
                      <div className="mt-1 text-sm font-medium">
                        {formatDate(currentUser.lastLoginAt)}
                      </div>
                    </div>
                  </div>
                </div>

                <div className="rounded-[1.5rem] border border-border/70 bg-background/80 p-4">
                  <div className="grid gap-3 sm:grid-cols-2">
                    <div>
                      <div className="text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
                        Session public ID
                      </div>
                      <div className="mt-1 break-all font-mono text-xs leading-6 text-muted-foreground">
                        {session.sessionId}
                      </div>
                    </div>
                    <div>
                      <div className="text-xs font-semibold tracking-[0.24em] text-muted-foreground uppercase">
                        API base URL
                      </div>
                      <div className="mt-1 break-all font-mono text-xs leading-6 text-muted-foreground">
                        {getApiBaseUrl()}
                      </div>
                    </div>
                  </div>
                  <div className="mt-4 grid gap-3 sm:grid-cols-2">
                    <div className="rounded-2xl border border-border/60 bg-muted/25 p-3">
                      <div className="text-[0.68rem] font-semibold tracking-[0.22em] text-muted-foreground uppercase">
                        Access token expiry
                      </div>
                      <div className="mt-1 text-sm">{formatDate(session.accessTokenExpiresAt)}</div>
                    </div>
                    <div className="rounded-2xl border border-border/60 bg-muted/25 p-3">
                      <div className="text-[0.68rem] font-semibold tracking-[0.22em] text-muted-foreground uppercase">
                        Refresh token expiry
                      </div>
                      <div className="mt-1 text-sm">{formatDate(session.refreshTokenExpiresAt)}</div>
                    </div>
                  </div>
                </div>

                <div className="flex flex-wrap gap-3">
                  <Button
                    size="lg"
                    onClick={handleRefreshProfile}
                    disabled={busyAction !== null}
                  >
                    {busyAction === "profile" ? "Refreshing..." : "Refresh profile"}
                  </Button>
                  <Button
                    size="lg"
                    variant="outline"
                    onClick={handleLogout}
                    disabled={busyAction !== null}
                  >
                    {busyAction === "logout" ? "Signing out..." : "Logout"}
                  </Button>
                </div>
              </div>
            </Panel>
          ) : null}

          {!session ? (
            <>
              {mode === "login" ? (
                <Panel
                  title="Resume a traveler session"
                  description="This signs into the local identity provider using email and password, then reloads iam.users through /auth/me."
                >
                  <form className="grid gap-4" onSubmit={handleLogin}>
                    <Field label="Email">
                      <Input
                        type="email"
                        autoComplete="email"
                        value={loginForm.email}
                        onChange={(event) =>
                          setLoginForm((current) => ({
                            ...current,
                            email: event.target.value,
                          }))
                        }
                        placeholder="traveler@bookyourstay.dev"
                        required
                      />
                    </Field>
                    <Field label="Password">
                      <Input
                        type="password"
                        autoComplete="current-password"
                        value={loginForm.password}
                        onChange={(event) =>
                          setLoginForm((current) => ({
                            ...current,
                            password: event.target.value,
                          }))
                        }
                        placeholder="Enter your password"
                        required
                      />
                    </Field>
                    <div className="rounded-2xl border border-border/60 bg-muted/25 p-4 text-sm leading-6 text-muted-foreground">
                      Login updates <span className="font-mono text-foreground">iam.users.last_login_at</span>, touches the local identity usage timestamp, and opens a new row in <span className="font-mono text-foreground">iam.user_sessions</span>.
                    </div>
                    <Button size="lg" className="mt-2" disabled={busyAction !== null}>
                      {busyAction === "login" ? "Signing in..." : "Sign in"}
                    </Button>
                  </form>
                </Panel>
              ) : null}

              {mode === "register" ? (
                <Panel
                  title="Create a traveler identity"
                  description="The current backend register flow only needs full name, email, and password. Phone and partner-role data exist in schema but are not part of this endpoint yet."
                >
                  <form className="grid gap-4" onSubmit={handleRegister}>
                    <Field label="Full name">
                      <Input
                        type="text"
                        autoComplete="name"
                        value={registerForm.fullName}
                        onChange={(event) =>
                          setRegisterForm((current) => ({
                            ...current,
                            fullName: event.target.value,
                          }))
                        }
                        placeholder="Taylor Morgan"
                        required
                      />
                    </Field>
                    <Field label="Email">
                      <Input
                        type="email"
                        autoComplete="email"
                        value={registerForm.email}
                        onChange={(event) =>
                          setRegisterForm((current) => ({
                            ...current,
                            email: event.target.value,
                          }))
                        }
                        placeholder="traveler@bookyourstay.dev"
                        required
                      />
                    </Field>
                    <Field label="Password" hint="8+ characters recommended">
                      <Input
                        type="password"
                        autoComplete="new-password"
                        value={registerForm.password}
                        onChange={(event) =>
                          setRegisterForm((current) => ({
                            ...current,
                            password: event.target.value,
                          }))
                        }
                        placeholder="Create a secure password"
                        required
                      />
                    </Field>
                    <div className="rounded-2xl border border-border/60 bg-muted/25 p-4 text-sm leading-6 text-muted-foreground">
                      Successful registration creates one traveler row in <span className="font-mono text-foreground">iam.users</span>, one local credential row in <span className="font-mono text-foreground">iam.user_identities</span>, and one active session in <span className="font-mono text-foreground">iam.user_sessions</span>.
                    </div>
                    <Button size="lg" className="mt-2" disabled={busyAction !== null}>
                      {busyAction === "register"
                        ? "Creating account..."
                        : "Create account"}
                    </Button>
                  </form>
                </Panel>
              ) : null}

              {mode === "recovery" ? (
                <div className="grid gap-5">
                  <Panel
                    title="Issue a password reset token"
                    description="This writes a recoverable token into iam.password_reset_tokens. In this dev flow, the backend returns the plaintext token so you can test the full loop."
                  >
                    <form className="grid gap-4" onSubmit={handleForgotPassword}>
                      <Field label="Account email">
                        <Input
                          type="email"
                          autoComplete="email"
                          value={forgotEmail}
                          onChange={(event) => setForgotEmail(event.target.value)}
                          placeholder="traveler@bookyourstay.dev"
                          required
                        />
                      </Field>
                      <Button size="lg" className="mt-2" disabled={busyAction !== null}>
                        {busyAction === "forgot"
                          ? "Requesting token..."
                          : "Request reset token"}
                      </Button>
                    </form>

                    {resetTicket ? (
                      <div className="mt-5 rounded-[1.5rem] border border-amber-200 bg-amber-50 p-4 text-sm text-amber-950">
                        <div className="flex items-center gap-2">
                          <div className="font-semibold">Latest reset token</div>
                          <Badge variant="warning">Dev only</Badge>
                        </div>
                        <div className="mt-2 break-all font-mono text-xs leading-6">
                          {resetTicket.resetToken}
                        </div>
                        <div className="mt-3 text-xs leading-5 opacity-80">
                          Expires at {formatDate(resetTicket.expiresAt)}
                        </div>
                      </div>
                    ) : null}
                  </Panel>

                  <Panel
                    title="Consume the reset token"
                    description="The reset endpoint validates the token, marks it consumed, and replaces the local password hash. New password validation currently enforces the backend minimum of 8 characters."
                  >
                    <form className="grid gap-4" onSubmit={handleResetPassword}>
                      <Field label="Reset token">
                        <Input
                          type="text"
                          value={resetForm.resetToken}
                          onChange={(event) =>
                            setResetForm((current) => ({
                              ...current,
                              resetToken: event.target.value,
                            }))
                          }
                          placeholder="Paste the token returned above"
                          required
                        />
                      </Field>
                      <Field label="New password">
                        <Input
                          type="password"
                          autoComplete="new-password"
                          value={resetForm.newPassword}
                          onChange={(event) =>
                            setResetForm((current) => ({
                              ...current,
                              newPassword: event.target.value,
                            }))
                          }
                          placeholder="Choose a new password"
                        required
                      />
                    </Field>
                    <div className="rounded-2xl border border-border/60 bg-muted/25 p-4 text-sm leading-6 text-muted-foreground">
                      This flow is intentionally scoped to local credentials. Email verification tokens, phone verification, device metadata, and partner access levels are schema-backed next steps, not part of the current API contract.
                    </div>
                      <Button size="lg" className="mt-2" disabled={busyAction !== null}>
                        {busyAction === "reset"
                          ? "Updating password..."
                          : "Reset password"}
                      </Button>
                    </form>
                  </Panel>
                </div>
              ) : null}
            </>
          ) : null}
        </section>
      </div>
    </main>
  )
}
