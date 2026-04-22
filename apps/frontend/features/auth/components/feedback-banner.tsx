"use client"

import {
  AlertCircle,
  CheckCircle,
  Info,
  XCircle,
} from "@hugeicons/core-free-icons"
import { HugeiconsIcon } from "@hugeicons/react"

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import type { FeedbackState } from "../types/auth"

interface FeedbackBannerProps {
  feedback: FeedbackState | null
}

export function FeedbackBanner({ feedback }: FeedbackBannerProps) {
  if (!feedback) return null

  const getIcon = () => {
    switch (feedback.tone) {
      case "success":
        return CheckCircle
      case "error":
        return XCircle
      case "warning":
        return AlertCircle
      case "info":
      default:
        return Info
    }
  }

  const getVariant = () => {
    switch (feedback.tone) {
      case "error":
        return "destructive"
      case "success":
      case "warning":
      case "info":
      default:
        return "default"
    }
  }

  return (
    <Alert variant={getVariant()} className="border">
      <HugeiconsIcon icon={getIcon()} />
      <AlertTitle>{feedback.title}</AlertTitle>
      {feedback.body && <AlertDescription>{feedback.body}</AlertDescription>}
    </Alert>
  )
}
