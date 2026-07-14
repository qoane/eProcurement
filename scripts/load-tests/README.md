# ProcuraFlow load tests

Plain PowerShell scripts are provided so Docker is not required. Run from this folder:

```powershell
./run-load-test.ps1 -BaseUrl http://localhost:5000 -Scenario public-tenders -Requests 100
```

Scenarios: public-tenders, login, dashboard, suppliers, tenders, bids, reporting.
Expected Phase 1 threshold: p95 below 1000 ms for simple reads in local/demo conditions; investigate failed requests and slow samples in /app/operations/performance.
