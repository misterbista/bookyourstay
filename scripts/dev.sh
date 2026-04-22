#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BACKEND_PORT="${BACKEND_PORT:-8080}"
FRONTEND_PORT="${FRONTEND_PORT:-3000}"

backend_pid=""
frontend_pid=""

kill_stale_processes() {
  pkill -f "dotnet watch --project apps/backend" 2>/dev/null || true
  pkill -f "next dev .*--port $FRONTEND_PORT" 2>/dev/null || true
}

stop_pid() {
  local pid="$1"

  if [[ -z "$pid" ]] || ! kill -0 "$pid" 2>/dev/null; then
    return
  fi

  kill -INT "$pid" 2>/dev/null || true
  sleep 1

  if kill -0 "$pid" 2>/dev/null; then
    kill "$pid" 2>/dev/null || true
  fi

  sleep 1

  if kill -0 "$pid" 2>/dev/null; then
    kill -9 "$pid" 2>/dev/null || true
  fi
}

kill_port() {
  local port="$1"
  local pids

  pids="$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)"
  if [[ -z "$pids" ]]; then
    return
  fi

  kill $pids 2>/dev/null || true
  sleep 1

  pids="$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)"
  if [[ -n "$pids" ]]; then
    kill -9 $pids 2>/dev/null || true
  fi
}

cleanup() {
  trap - EXIT INT TERM

  stop_pid "$backend_pid"
  stop_pid "$frontend_pid"

  if [[ -n "$backend_pid" ]]; then
    wait "$backend_pid" 2>/dev/null || true
  fi

  if [[ -n "$frontend_pid" ]]; then
    wait "$frontend_pid" 2>/dev/null || true
  fi

  kill_port "$BACKEND_PORT"
  kill_port "$FRONTEND_PORT"
}

trap cleanup EXIT INT TERM

cd "$ROOT_DIR"

kill_stale_processes
kill_port "$BACKEND_PORT"
kill_port "$FRONTEND_PORT"

docker compose -f compose.yml up -d --remove-orphans postgres

dotnet watch --project apps/backend run --urls "http://localhost:$BACKEND_PORT" &
backend_pid="$!"

bun run --filter frontend dev -- --hostname 0.0.0.0 --port "$FRONTEND_PORT" &
frontend_pid="$!"

while kill -0 "$backend_pid" 2>/dev/null && kill -0 "$frontend_pid" 2>/dev/null; do
  sleep 1
done
