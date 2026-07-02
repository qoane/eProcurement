# LCA eProcurement

Production-shaped demo foundation for the Lesotho Communications Authority eProcurement platform.

- Modular monolith backend foundation with WorkflowEngine, BusinessRulesEngine, AuditEngine and SupplierManagement vertical slice.
- SQL Server is the local demo database target: `Server=localhost;Database=LcaEProcurement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`.
- Tests are designed to use in-memory/SQLite-safe paths and must not require Docker or a manually running SQL Server.
- React + TypeScript + Vite frontend with an enterprise supplier onboarding dashboard.

## Automation

```bash
bash scripts/task.sh health
bash scripts/task.sh setup
bash scripts/task.sh ci
```

No repository script uses Docker.
