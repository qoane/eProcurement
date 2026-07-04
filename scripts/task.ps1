param([string]$Task = "health")
$Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
function Has-DotNet { $null -ne (Get-Command dotnet -ErrorAction SilentlyContinue) }
function Require-DotNet { if (-not (Has-DotNet)) { throw "dotnet SDK is required for '$Task'" } }
function In-Backend([scriptblock]$Block) { Push-Location "$Root/backend"; try { & $Block } finally { Pop-Location } }
switch ($Task) {
  "setup" { Push-Location "$Root/frontend"; npm install; Pop-Location; Require-DotNet; In-Backend { dotnet restore Lca.EProcurement.sln } }
  "ci" { & $MyInvocation.MyCommand.Path health; Require-DotNet; In-Backend { dotnet restore Lca.EProcurement.sln }; In-Backend { dotnet build Lca.EProcurement.sln --no-restore }; In-Backend { dotnet test Lca.EProcurement.sln --no-build }; Push-Location "$Root/frontend"; npm run build; Pop-Location }
  "test" { Require-DotNet; In-Backend { dotnet test Lca.EProcurement.sln } }
  "migrate" { Require-DotNet; In-Backend { dotnet ef database update --project src/Lca.EProcurement.EntityFrameworkCore --startup-project src/Lca.EProcurement.Api } }
  "seed" { Require-DotNet; In-Backend { dotnet run --project src/Lca.EProcurement.Api -- --seed } }
  "reset-db" { Require-DotNet; In-Backend { dotnet ef database drop --force --project src/Lca.EProcurement.EntityFrameworkCore --startup-project src/Lca.EProcurement.Api }; & $MyInvocation.MyCommand.Path migrate; & $MyInvocation.MyCommand.Path seed }
  "format" { Push-Location "$Root/frontend"; npx prettier --write src; Pop-Location; Require-DotNet; In-Backend { dotnet format Lca.EProcurement.sln --no-restore } }
  "clean" { Remove-Item -Recurse -Force "$Root/frontend/dist","$Root/frontend/node_modules/.vite" -ErrorAction SilentlyContinue; Require-DotNet; In-Backend { dotnet clean Lca.EProcurement.sln } }
  "health" { Write-Host "LCA eProcurement health"; node --version; npm --version; if (Has-DotNet) { dotnet --info } else { Write-Warning "dotnet SDK not found" }; if (Get-Command rg -ErrorAction SilentlyContinue) { $docker = rg -n '(^|[;&|[:space:]])docker([[:space:]]|$)|docker-compose[[:space:]]' "$Root/scripts"; if ($LASTEXITCODE -eq 0) { throw "Docker command found in automation: $docker" } } }
  "dev-api" { Require-DotNet; In-Backend { dotnet run --project src/Lca.EProcurement.Api } }
  "dev-web" { Push-Location "$Root/frontend"; npm run dev; Pop-Location }
  default { throw "Unknown task $Task" }
}
