"use client"

import { type FormEvent, useEffect, useState } from "react"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Separator } from "@/components/ui/separator"
import {
  ApiClientError,
  type AuthSession,
  type CurrentUser,
  type PasswordResetTicket,
  forgotPassword,
  getCurrentUser,
  login,
  logout,
  register,
  resetPassword,
} from "@/lib/auth-api"
import { cn } from "@/lib/utils"
import { readStoredSession, persistSession } from "@/features/auth/lib/session-storage"
import { BrandLogo } from "@/shared/components/brand-logo"
import { ThemeToggleButton } from "@/shared/components/theme-toggle-button"

type AuthMode = "login" | "register" | "recovery"

type FeedbackState = {
  tone: "success" | "error" | "info"
  title: string
  body: string
} | null

function flattenErrors(error: ApiClientError) {
  const details = Object.values(error.errors).flat().filter(Boolean)

  return details.length > 0 ? details.join(" ") : error.message
}

function formatDate(value: string | null) {
  if (!value) return "Not available"

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value))
}

function Field({
  label,
  children,
  hint,
}: {
  label: string
  children: React.ReactNode
  hint?: string
}) {
  return (
    <label className="flex flex-col gap-1.5">
      <span className="flex items-center justify-between text-[0.8rem] font-medium text-foreground/80">
        <span>{label}</span>
        {hint ? (
          <span className="text-xs text-muted-foreground">{hint}</span>
        ) : null}
      </span>
      {children}
    </label>
  )
}

function FeedbackBanner({
  feedback,
}: {
  feedback: NonNullable<FeedbackState>
}) {
  const toneClasses = {
    success:
      "border-emerald-200/60 bg-emerald-50/80 text-emerald-900 dark:border-emerald-500/20 dark:bg-emerald-950/40 dark:text-emerald-200",
    error:
      "border-destructive/20 bg-destructive/5 text-destructive dark:border-destructive/30 dark:bg-destructive/10",
    info: "border-border bg-muted/60 text-foreground",
  }

  return (
    <div
      className={cn(
        "rounded-xl border px-4 py-3 text-sm",
        toneClasses[feedback.tone],
      )}
    >
      <div className="font-semibold">{feedback.title}</div>
      <p className="mt-0.5 leading-relaxed opacity-90">{feedback.body}</p>
    </div>
  )
}

function AuthWorkspace() {
  const [mode, setMode] = useState<AuthMode>("login")
  const [session, setSession] = useState<AuthSession | null>(null)
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null)
  const [isHydratingSession, setIsHydratingSession] = useState(true)
  const [busyAction, setBusyAction] = useState<string | null>(null)
  const [feedback, setFeedback] = useState<FeedbackState>(null)
  const [resetTicket, setResetTicket] = useState<PasswordResetTicket | null>(
    null,
  )

  const [registerForm, setRegisterForm] = useState({
    fullName: "",
    email: "",
    password: "",
  })
  const [loginForm, setLoginForm] = useState({ email: "", password: "" })
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
      setRegisterForm({ fullName: "", email: "", password: "" })
      setFeedback({
        tone: "success",
        title: "Account created",
        body: "Your account is ready. You can continue from here.",
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
        body: "Welcome back to BookYourStay.",
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

  async function handleForgotPassword(event: FormEvent<HTMLFormElement>) {
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
        body: "Use the token below to set a new password.",
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
        body: "You can now sign in with your new password.",
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
    } finally {
      persistSession(null)
      setSession(null)
      setCurrentUser(null)
      setBusyAction(null)
      setMode("login")
    }
  }

  const inputClasses =
    "h-12 rounded-xl border-border/70 bg-background px-4 text-sm shadow-none placeholder:text-muted-foreground/60 focus-visible:border-primary/40 focus-visible:ring-primary/20"

  return (
    <main className="min-h-svh bg-background">
      <div className="mx-auto flex min-h-svh w-full max-w-7xl flex-col">
        <header className="flex items-center justify-between px-6 py-4 sm:px-10">
          <BrandLogo
            className="h-20 w-72 sm:h-24 sm:w-[24rem]"
            sizes="(min-width: 640px) 384px, 288px"
            priority
          />
          <ThemeToggleButton
            variant="outline"
            className="rounded-xl border-border/60 bg-background/80 text-muted-foreground hover:bg-muted hover:text-foreground"
          />
        </header>

        <div className="flex flex-1 items-center justify-center px-6 pb-10 sm:px-10">
          <div className="w-full max-w-[460px] space-y-8">
            {isHydratingSession ? (
              <div className="space-y-4">
                <div className="h-8 w-48 animate-pulse rounded-lg bg-muted" />
                <div className="h-5 w-64 animate-pulse rounded-md bg-muted/60" />
                <div className="mt-6 space-y-3">
                  <div className="h-12 animate-pulse rounded-xl bg-muted/50" />
                  <div className="h-12 animate-pulse rounded-xl bg-muted/50" />
                  <div className="h-12 animate-pulse rounded-xl bg-muted/40" />
                </div>
              </div>
            ) : session && currentUser ? (
              <div className="space-y-6">
                <div>
                  <Badge variant="success" className="mb-3">
                    {currentUser.status}
                  </Badge>
                  <h2 className="text-3xl font-semibold tracking-tight">
                    Welcome, {currentUser.fullName}
                  </h2>
                  <p className="mt-2 text-sm leading-relaxed text-muted-foreground">
                    Signed in as {currentUser.email}
                  </p>
                  <p className="mt-1 text-sm text-muted-foreground">
                    Last login: {formatDate(currentUser.lastLoginAt)}
                  </p>
                </div>

                {feedback ? <FeedbackBanner feedback={feedback} /> : null}

                <Button
                  size="lg"
                  variant="outline"
                  className="h-12 w-full rounded-xl"
                  onClick={handleLogout}
                  disabled={busyAction !== null}
                >
                  {busyAction === "logout" ? "Signing out..." : "Sign out"}
                </Button>
              </div>
            ) : mode === "recovery" ? (
              <div className="space-y-6">
                <div>
                  <h2 className="text-2xl font-semibold tracking-tight sm:text-3xl">
                    Reset your password
                  </h2>
                  <p className="mt-2 text-sm leading-relaxed text-muted-foreground">
                    Enter your email to receive a reset token.
                  </p>
                </div>

                {feedback ? <FeedbackBanner feedback={feedback} /> : null}

                <form className="space-y-4" onSubmit={handleForgotPassword}>
                  <Field label="Email address">
                    <Input
                      type="email"
                      autoComplete="email"
                      value={forgotEmail}
                      onChange={(e) => setForgotEmail(e.target.value)}
                      placeholder="traveler@bookyourstay.dev"
                      required
                      className={inputClasses}
                    />
                  </Field>

                  <Button
                    size="lg"
                    className="h-12 w-full rounded-xl text-sm font-semibold"
                    disabled={busyAction !== null}
                  >
                    {busyAction === "forgot"
                      ? "Requesting token..."
                      : "Request reset token"}
                  </Button>
                </form>

                {resetTicket ? (
                  <div className="rounded-xl border border-primary/20 bg-primary/5 p-4">
                    <div className="mb-2 flex items-center gap-2">
                      <span className="text-sm font-semibold">Reset token</span>
                      <Badge variant="warning">Dev only</Badge>
                    </div>
                    <p className="break-all font-mono text-xs text-muted-foreground">
                      {resetTicket.resetToken}
                    </p>
                  </div>
                ) : null}

                <Separator />

                <form className="space-y-4" onSubmit={handleResetPassword}>
                  <Field label="Reset token">
                    <Input
                      value={resetForm.resetToken}
                      onChange={(e) =>
                        setResetForm((current) => ({
                          ...current,
                          resetToken: e.target.value,
                        }))
                      }
                      placeholder="Paste reset token"
                      required
                      className={inputClasses}
                    />
                  </Field>

                  <Field label="New password" hint="Minimum 8 characters">
                    <Input
                      type="password"
                      autoComplete="new-password"
                      value={resetForm.newPassword}
                      onChange={(e) =>
                        setResetForm((current) => ({
                          ...current,
                          newPassword: e.target.value,
                        }))
                      }
                      placeholder="Choose a new password"
                      required
                      className={inputClasses}
                    />
                  </Field>

                  <Button
                    size="lg"
                    variant="outline"
                    className="h-12 w-full rounded-xl text-sm font-semibold"
                    disabled={busyAction !== null}
                  >
                    {busyAction === "reset" ? "Updating password..." : "Reset password"}
                  </Button>
                </form>

                <Button
                  variant="ghost"
                  className="w-full rounded-xl"
                  onClick={() => setMode("login")}
                >
                  Back to sign in
                </Button>
              </div>
            ) : (
              <div className="space-y-6">
                <div className="space-y-2">
                  <h2 className="text-3xl font-semibold tracking-tight">
                    {mode === "login" ? "Welcome back" : "Create your account"}
                  </h2>
                  <p className="text-sm leading-relaxed text-muted-foreground">
                    {mode === "login"
                      ? "Sign in to manage bookings, saved stays, and upcoming trips."
                      : "Create your traveler account to start booking beautiful stays."}
                  </p>
                </div>

                {feedback ? <FeedbackBanner feedback={feedback} /> : null}

                {mode === "login" ? (
                  <form className="space-y-4" onSubmit={handleLogin}>
                    <Field label="Email address">
                      <Input
                        type="email"
                        autoComplete="email"
                        value={loginForm.email}
                        onChange={(e) =>
                          setLoginForm((current) => ({
                            ...current,
                            email: e.target.value,
                          }))
                        }
                        placeholder="traveler@bookyourstay.dev"
                        required
                        className={inputClasses}
                      />
                    </Field>

                    <Field label="Password">
                      <Input
                        type="password"
                        autoComplete="current-password"
                        value={loginForm.password}
                        onChange={(e) =>
                          setLoginForm((current) => ({
                            ...current,
                            password: e.target.value,
                          }))
                        }
                        placeholder="Enter your password"
                        required
                        className={inputClasses}
                      />
                    </Field>

                    <Button
                      size="lg"
                      className="h-12 w-full rounded-xl text-sm font-semibold"
                      disabled={busyAction !== null}
                    >
                      {busyAction === "login" ? "Signing in..." : "Sign in"}
                    </Button>
                  </form>
                ) : (
                  <form className="space-y-4" onSubmit={handleRegister}>
                    <Field label="Full name">
                      <Input
                        autoComplete="name"
                        value={registerForm.fullName}
                        onChange={(e) =>
                          setRegisterForm((current) => ({
                            ...current,
                            fullName: e.target.value,
                          }))
                        }
                        placeholder="Taylor Morgan"
                        required
                        className={inputClasses}
                      />
                    </Field>

                    <Field label="Email address">
                      <Input
                        type="email"
                        autoComplete="email"
                        value={registerForm.email}
                        onChange={(e) =>
                          setRegisterForm((current) => ({
                            ...current,
                            email: e.target.value,
                          }))
                        }
                        placeholder="traveler@bookyourstay.dev"
                        required
                        className={inputClasses}
                      />
                    </Field>

                    <Field label="Password" hint="Minimum 8 characters">
                      <Input
                        type="password"
                        autoComplete="new-password"
                        value={registerForm.password}
                        onChange={(e) =>
                          setRegisterForm((current) => ({
                            ...current,
                            password: e.target.value,
                          }))
                        }
                        placeholder="Create a secure password"
                        required
                        className={inputClasses}
                      />
                    </Field>

                    <Button
                      size="lg"
                      className="h-12 w-full rounded-xl text-sm font-semibold"
                      disabled={busyAction !== null}
                    >
                      {busyAction === "register"
                        ? "Creating account..."
                        : "Create account"}
                    </Button>
                  </form>
                )}

                <Separator />

                <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
                  <Button
                    variant="ghost"
                    className="rounded-xl"
                    onClick={() =>
                      setMode((current) =>
                        current === "login" ? "register" : "login",
                      )
                    }
                  >
                    {mode === "login"
                      ? "Need an account? Sign up"
                      : "Already have an account? Sign in"}
                  </Button>

                  <Button
                    variant="ghost"
                    className="rounded-xl text-muted-foreground"
                    onClick={() => setMode("recovery")}
                  >
                    Forgot password?
                  </Button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </main>
  )
}

export { AuthWorkspace }
