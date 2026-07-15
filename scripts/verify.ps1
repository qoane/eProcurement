$ErrorActionPreference = "Stop"
$RootDir = Split-Path -Parent $PSScriptRoot
Set-Location $RootDir

dotnet restore backend/Lca.EProcurement.sln
dotnet build backend/Lca.EProcurement.sln --no-restore
dotnet test backend/Lca.EProcurement.sln --no-build
Set-Location frontend
npm install
npm run build
