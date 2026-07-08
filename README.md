# LCA eProcurement

Production-shaped demo foundation for the Lesotho Communications Authority eProcurement platform.

- Modular monolith backend foundation with WorkflowEngine, BusinessRulesEngine, AuditEngine and SupplierManagement vertical slice.
- Backend projects target .NET 10 (`net10.0`).
- SQL Server LocalDB is the local application database target: `Server=(localdb)\MSSQLLocalDB;Database=LcaEProcurement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`.
- EF Core migrations and idempotent seed data prepare the demo database without Docker.
- React + TypeScript + Vite frontend with an enterprise supplier onboarding dashboard.

## Windows quick start

Run these commands from the repository root in PowerShell:

```powershell
pwsh scripts/task.ps1 health
pwsh scripts/task.ps1 setup
pwsh scripts/task.ps1 reset-db
pwsh scripts/task.ps1 dev-api
```

In a second PowerShell terminal, start the frontend:

```powershell
pwsh scripts/task.ps1 dev-web
```

Development API URLs:

- Landing endpoint: `https://localhost:<port>/`
- Health endpoint: `https://localhost:<port>/health`
- Swagger UI: `https://localhost:<port>/swagger`
- Suppliers API: `https://localhost:<port>/api/suppliers`

## Automation

Bash:

```bash
bash scripts/task.sh health
bash scripts/task.sh setup
bash scripts/task.sh reset-db
bash scripts/task.sh ci
```

PowerShell:

```powershell
pwsh scripts/task.ps1 health
pwsh scripts/task.ps1 setup
pwsh scripts/task.ps1 reset-db
pwsh scripts/task.ps1 ci
```

Available tasks are `health`, `setup`, `ci`, `test`, `migrate`, `seed`, `reset-db`, `format`, `clean`, `dev-api`, and `dev-web`.
No repository script uses Docker.

## Embedding Latest Opportunities

The public Latest Opportunities widget is available without authentication and is designed for embedding on the LCA website.

### JavaScript placeholder snippet

Use this markup on the host page when the production JavaScript loader is published:

```html
<div id="lca-opportunities-widget"></div>
<script src="https://procurement.lca.org.ls/widgets/latest-opportunities.js"></script>
```

### Iframe-ready widget

Until the JavaScript loader is published, embed the compact widget route directly with an iframe:

```html
<iframe
  src="https://procurement.lca.org.ls/widgets/latest-opportunities"
  title="Latest LCA procurement opportunities"
  style="width: 100%; max-width: 540px; min-height: 420px; border: 0;"
></iframe>
```

The widget reads from `GET /api/public/widgets/latest-tenders?take=5`, which returns the tender number, title, closing date, category, status, and public opportunity URL for the latest visible public tenders.
