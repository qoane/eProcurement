#!/usr/bin/env bash
set -euo pipefail
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"
dotnet restore backend/Lca.EProcurement.sln
dotnet build backend/Lca.EProcurement.sln --no-restore
dotnet test backend/Lca.EProcurement.sln --no-build
cd frontend
npm install
npm run build
