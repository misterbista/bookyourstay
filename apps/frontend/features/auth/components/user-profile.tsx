"use client"

import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"
import { ApiClientError, logout } from "../api/auth-api"
import type { CurrentUser, FeedbackState } from "../types/auth"

interface UserProfileProps {
  user: CurrentUser
  onLogout: () => void
  setFeedback: (feedback: FeedbackState) => void
  isSubmitting: boolean
  setIsSubmitting: (submitting: boolean) => void
}

export function UserProfile({
  user,
  onLogout,
  setFeedback,
  isSubmitting,
  setIsSubmitting,
}: UserProfileProps) {
  const handleLogout = async () => {
    setIsSubmitting(true)

    try {
      await logout()

      setFeedback({
        tone: "info",
        title: "Logged out",
        body: "You have been successfully logged out.",
      })

      onLogout()
    } catch (error) {
      setFeedback({
        tone: "error",
        title: "Logout failed",
        body:
          error instanceof ApiClientError
            ? error.message
            : "An unexpected error occurred.",
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="text-center">
        <h2 className="text-xl font-semibold">
          Welcome back, {user.fullName}!
        </h2>
        <p className="text-sm text-muted-foreground">
          You are currently logged in.
        </p>
      </div>

      <div className="space-y-4">
        <div className="space-y-3">
          <div>
            <label className="text-sm font-medium">Full Name</label>
            <p className="text-sm text-muted-foreground">{user.fullName}</p>
          </div>

          <div>
            <label className="text-sm font-medium">Email</label>
            <p className="text-sm text-muted-foreground">{user.email}</p>
          </div>

          <div>
            <label className="text-sm font-medium">Status</label>
            <Badge
              variant={user.status === "active" ? "default" : "secondary"}
              className="mt-1"
            >
              {user.status}
            </Badge>
          </div>

          <div>
            <label className="text-sm font-medium">Account Created</label>
            <p className="text-sm text-muted-foreground">
              {formatDate(user.createdAt)}
            </p>
          </div>

          <div>
            <label className="text-sm font-medium">Last Login</label>
            <p className="text-sm text-muted-foreground">
              {formatDate(user.lastLoginAt)}
            </p>
          </div>
        </div>

        <Separator />

        <Button
          onClick={handleLogout}
          variant="outline"
          className="w-full"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Logging out..." : "Logout"}
        </Button>
      </div>
    </div>
  )
}

function formatDate(value: string | null) {
  if (!value) return "Not available"

  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value))
}
