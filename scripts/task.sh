#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cmd="${1:-health}"
DOTNET_MISSING=0
has_dotnet(){ command -v dotnet >/dev/null 2>&1; }
run_dotnet(){ if has_dotnet; then (cd "$ROOT/backend" && dotnet "$@"); else echo "WARNING: dotnet SDK not found; skipping dotnet $*"; DOTNET_MISSING=1; fi }
case "$cmd" in
  setup) (cd "$ROOT/frontend" && npm install); run_dotnet restore Lca.EProcurement.sln ;;
  ci) "$0" health; "$0" format; "$0" test; (cd "$ROOT/frontend" && npm run build) ;;
  test) run_dotnet test Lca.EProcurement.sln --no-restore ;;
  migrate) run_dotnet ef database update --project src/Lca.EProcurement.EntityFrameworkCore --startup-project src/Lca.EProcurement.Api ;;
  seed) echo "Seed data is defined in backend/src/Lca.EProcurement.EntityFrameworkCore/SeedData.cs and applied by the API/migration path when a database is available." ;;
  reset-db) echo "Reset SQL Server database LcaEProcurement, then run migrate and seed; no Docker is used." ;;
  format) (cd "$ROOT/frontend" && npx prettier --write src >/dev/null); if has_dotnet; then (cd "$ROOT/backend" && dotnet format --verify-no-changes --no-restore || dotnet format --no-restore); else echo "WARNING: dotnet SDK not found; skipping dotnet format"; fi ;;
  clean) rm -rf "$ROOT/frontend/dist" "$ROOT/frontend/node_modules/.vite"; run_dotnet clean Lca.EProcurement.sln ;;
  health) echo "LCA eProcurement health"; node --version; npm --version; if has_dotnet; then dotnet --info | head -40; else echo "WARNING: dotnet SDK not found in this environment"; fi; if rg -n "(^|[;&|[:space:]])docker([[:space:]]|$)|docker-compose[[:space:]]" "$ROOT/scripts" >/dev/null; then echo "ERROR: Docker command found in automation"; exit 1; fi ;;
  dev-api) run_dotnet run --project src/Lca.EProcurement.Api ;;
  dev-web) (cd "$ROOT/frontend" && npm run dev) ;;
  *) echo "Usage: $0 {setup|ci|test|migrate|seed|reset-db|format|clean|health|dev-api|dev-web}"; exit 2 ;;
esac
