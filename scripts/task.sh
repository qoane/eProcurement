#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cmd="${1:-health}"
has_dotnet(){ command -v dotnet >/dev/null 2>&1; }
require_dotnet(){ if ! has_dotnet; then echo "ERROR: dotnet SDK is required for '$cmd'" >&2; exit 1; fi }
backend(){ (cd "$ROOT/backend" && "$@"); }
case "$cmd" in
  setup) (cd "$ROOT/frontend" && npm install); require_dotnet; backend dotnet restore Lca.EProcurement.sln ;;
  ci) "$0" health; require_dotnet; backend dotnet restore Lca.EProcurement.sln; backend dotnet build Lca.EProcurement.sln --no-restore; backend dotnet test Lca.EProcurement.sln --no-build; (cd "$ROOT/frontend" && npm run build) ;;
  test) require_dotnet; backend dotnet test Lca.EProcurement.sln ;;
  migrate) require_dotnet; backend dotnet ef database update --project src/Lca.EProcurement.EntityFrameworkCore --startup-project src/Lca.EProcurement.Api ;;
  seed) require_dotnet; backend dotnet run --project src/Lca.EProcurement.Api -- --seed ;;
  reset-db) require_dotnet; backend dotnet ef database drop --force --project src/Lca.EProcurement.EntityFrameworkCore --startup-project src/Lca.EProcurement.Api; "$0" migrate; "$0" seed ;;
  format) (cd "$ROOT/frontend" && npx prettier --write src); require_dotnet; backend dotnet format Lca.EProcurement.sln --no-restore ;;
  clean) rm -rf "$ROOT/frontend/dist" "$ROOT/frontend/node_modules/.vite"; require_dotnet; backend dotnet clean Lca.EProcurement.sln ;;
  health) echo "LCA eProcurement health"; node --version; npm --version; if has_dotnet; then dotnet --info | head -80; else echo "WARNING: dotnet SDK not found"; fi; if rg -n '(^|[;&|[:space:]])docker([[:space:]]|$)|docker-compose[[:space:]]' "$ROOT/scripts" >/dev/null; then echo "ERROR: Docker command found in automation"; exit 1; fi ;;
  dev-api) require_dotnet; backend dotnet run --project src/Lca.EProcurement.Api ;;
  dev-web) (cd "$ROOT/frontend" && npm run dev) ;;
  *) echo "Usage: $0 {setup|ci|test|migrate|seed|reset-db|format|clean|health|dev-api|dev-web}"; exit 2 ;;
esac
