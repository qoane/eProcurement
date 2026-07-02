param([string]$Task = "health")
$Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
function Has-DotNet { $null -ne (Get-Command dotnet -ErrorAction SilentlyContinue) }
switch ($Task) {
  "setup" { Push-Location "$Root/frontend"; npm install; Pop-Location; if (Has-DotNet) { Push-Location "$Root/backend"; dotnet restore Lca.EProcurement.sln; Pop-Location } else { Write-Warning "dotnet SDK not found" } }
  "ci" { & $MyInvocation.MyCommand.Path health; & $MyInvocation.MyCommand.Path format; & $MyInvocation.MyCommand.Path test; Push-Location "$Root/frontend"; npm run build; Pop-Location }
  "test" { if (Has-DotNet) { Push-Location "$Root/backend"; dotnet test Lca.EProcurement.sln --no-restore; Pop-Location } else { Write-Warning "dotnet SDK not found; skipping tests" } }
  "migrate" { if (Has-DotNet) { Push-Location "$Root/backend"; dotnet ef database update --project src/Lca.EProcurement.EntityFrameworkCore --startup-project src/Lca.EProcurement.Api; Pop-Location } }
  "seed" { Write-Host "Seed data is defined in backend/src/Lca.EProcurement.EntityFrameworkCore/SeedData.cs" }
  "reset-db" { Write-Host "Reset SQL Server database LcaEProcurement, then run migrate and seed; no Docker is used." }
  "format" { Push-Location "$Root/frontend"; npx prettier --write src; Pop-Location; if (Has-DotNet) { Push-Location "$Root/backend"; dotnet format --no-restore; Pop-Location } }
  "clean" { Remove-Item -Recurse -Force "$Root/frontend/dist","$Root/frontend/node_modules/.vite" -ErrorAction SilentlyContinue; if (Has-DotNet) { Push-Location "$Root/backend"; dotnet clean Lca.EProcurement.sln; Pop-Location } }
  "health" { Write-Host "LCA eProcurement health"; node --version; npm --version; if (Has-DotNet) { dotnet --info } else { Write-Warning "dotnet SDK not found" } }
  "dev-api" { if (Has-DotNet) { Push-Location "$Root/backend"; dotnet run --project src/Lca.EProcurement.Api; Pop-Location } }
  "dev-web" { Push-Location "$Root/frontend"; npm run dev; Pop-Location }
  default { throw "Unknown task $Task" }
}
