"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { SiteNavbar } from "@/shared/layout/site-navbar"
import { cn } from "@/lib/utils"
import { useAuth } from "../hooks/use-auth"
import { FeedbackBanner } from "./feedback-banner"
import { LoginForm } from "./login-form"
import { RegisterForm } from "./register-form"
import { UserProfile } from "./user-profile"

const AUTH_COPY = {
  login: {
    title: "Sign in to your account",
    description: "Welcome back! Please sign in to continue.",
  },
  register: {
    title: "Create your account",
    description: "Join BookYourStay to start planning your trips.",
  },
} as const

export function AuthWorkspace() {
  const {
    authMode,
    setAuthMode,
    currentUser,
    feedback,
    setFeedback,
    clearFeedback,
    isLoading,
    setCurrentUser,
    loadCurrentUser,
  } = useAuth()

  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleAuthSuccess = async () => {
    await loadCurrentUser()
  }

  const handleLogout = () => {
    setCurrentUser(null)
    setAuthMode("login")
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-background">
        <SiteNavbar variant="auth" />
        <div className="mx-auto flex min-h-[calc(100vh-72px)] max-w-md items-center justify-center px-4 py-10">
          <div className="text-center">
            <div className="mx-auto mb-4 h-8 w-8 animate-spin rounded-full border-b-2 border-primary"></div>
            <p className="text-muted-foreground">Loading...</p>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-background">
      <SiteNavbar variant="auth" />

      <div className="mx-auto flex min-h-[calc(100vh-72px)] max-w-md items-center justify-center px-4 py-10">
        <div className="w-full space-y-6">
          {feedback && <FeedbackBanner feedback={feedback} />}

          <div className={cn("space-y-6", feedback && "mt-6")}>
            {currentUser ? (
              <UserProfile
                user={currentUser}
                onLogout={handleLogout}
                setFeedback={setFeedback}
                isSubmitting={isSubmitting}
                setIsSubmitting={setIsSubmitting}
              />
            ) : (
              <>
                <div className="text-center">
                  <h1 className="text-2xl font-semibold">
                    {AUTH_COPY[authMode].title}
                  </h1>
                  <p className="mt-2 text-sm text-muted-foreground">
                    {AUTH_COPY[authMode].description}
                  </p>
                </div>

                <div className="flex rounded-lg border border-border p-1">
                  <Button
                    type="button"
                    variant={authMode === "login" ? "default" : "ghost"}
                    className="flex-1 rounded-md"
                    onClick={() => {
                      setAuthMode("login")
                      clearFeedback()
                    }}
                  >
                    Sign in
                  </Button>
                  <Button
                    type="button"
                    variant={authMode === "register" ? "default" : "ghost"}
                    className="flex-1 rounded-md"
                    onClick={() => {
                      setAuthMode("register")
                      clearFeedback()
                    }}
                  >
                    Sign up
                  </Button>
                </div>

                {authMode === "login" && (
                  <LoginForm
                    onSuccess={handleAuthSuccess}
                    setFeedback={setFeedback}
                    isSubmitting={isSubmitting}
                    setIsSubmitting={setIsSubmitting}
                  />
                )}

                {authMode === "register" && (
                  <RegisterForm
                    onSuccess={handleAuthSuccess}
                    setFeedback={setFeedback}
                    isSubmitting={isSubmitting}
                    setIsSubmitting={setIsSubmitting}
                  />
                )}
                <div className="text-center text-xs text-muted-foreground">
                  {authMode === "login" && (
                    <>
                      Don&apos;t have an account?{" "}
                      <Button
                        type="button"
                        variant="link"
                        className="h-auto p-0 text-xs"
                        onClick={() => {
                          setAuthMode("register")
                          clearFeedback()
                        }}
                      >
                        Sign up
                      </Button>
                    </>
                  )}

                  {authMode === "register" && (
                    <>
                      Already have an account?{" "}
                      <Button
                        type="button"
                        variant="link"
                        className="h-auto p-0 text-xs"
                        onClick={() => {
                          setAuthMode("login")
                          clearFeedback()
                        }}
                      >
                        Sign in
                      </Button>
                    </>
                  )}
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
