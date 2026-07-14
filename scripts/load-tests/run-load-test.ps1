param([string]$BaseUrl="http://localhost:5000",[string]$Scenario="public-tenders",[int]$Requests=50,[string]$Token="")
$paths=@{ "public-tenders"="/api/public/tenders"; "login"="/api/auth/login"; "dashboard"="/api/reporting/executive-dashboard"; "suppliers"="/api/suppliers"; "tenders"="/api/tenders"; "bids"="/api/bids"; "reporting"="/api/reporting/executive-dashboard" }
$path=$paths[$Scenario]; if(-not $path){ throw "Unknown scenario $Scenario" }
$headers=@{}; if($Token){ $headers.Authorization="Bearer $Token" }
$times=@(); $fail=0
1..$Requests | ForEach-Object { $sw=[Diagnostics.Stopwatch]::StartNew(); try { Invoke-WebRequest -Uri "$BaseUrl$path" -Headers $headers -UseBasicParsing | Out-Null } catch { $fail++ } finally { $sw.Stop(); $times += $sw.ElapsedMilliseconds } }
$sorted=$times | Sort-Object; $p95=$sorted[[Math]::Min($sorted.Count-1,[int]($sorted.Count*.95))]
[pscustomobject]@{ Scenario=$Scenario; Requests=$Requests; Failed=$fail; AverageMs=($times|Measure-Object -Average).Average; P95Ms=$p95 }
