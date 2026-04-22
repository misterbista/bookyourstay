"use client"

import { useCallback, useEffect, useState } from "react"
import { ApiClientError, getCurrentUser } from "../api/auth-api"
import type { AuthMode, CurrentUser, FeedbackState } from "../types/auth"

export function useAuth() {
  const [authMode, setAuthMode] = useState<AuthMode>("login")
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null)
  const [feedback, setFeedback] = useState<FeedbackState | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const loadCurrentUser = useCallback(async () => {
    try {
      const result = await getCurrentUser()
      setCurrentUser(result.data)
      setFeedback({
        tone: "success",
        title: "Welcome back!",
        body: `Hello, ${result.data.fullName}!`,
      })
    } catch (error) {
      if (error instanceof ApiClientError && error.status === 401) {
        setCurrentUser(null)
      } else {
        setFeedback({
          tone: "error",
          title: "Failed to load user",
          body: "Unable to verify your authentication status.",
        })
      }
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    void loadCurrentUser()
  }, [loadCurrentUser])

  const clearFeedback = () => setFeedback(null)

  return {
    authMode,
    setAuthMode,
    currentUser,
    setCurrentUser,
    feedback,
    setFeedback,
    clearFeedback,
    isLoading,
    loadCurrentUser,
  }
}
