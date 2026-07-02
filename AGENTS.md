# LCA eProcurement Agent Guide

This repository is a production-shaped demo eProcurement platform for the Lesotho Communications Authority.

## Architecture
- Modular monolith; do not introduce microservices.
- Backend: .NET 10 / ASP.NET Core style, ABP-ready modules, EF Core LINQ.
- Primary local demo database: Microsoft SQL Server.
- Keep provider configuration isolated in infrastructure so PostgreSQL can be added later.
- Do not put workflow or rule logic in the database.
- Do not use Docker or require Docker in scripts, tests, migrations, seed, or development.

## Modules
Platform modules: WorkflowEngine, BusinessRulesEngine, AuditEngine, FormConfigurationEngine, DocumentEngine, NotificationEngine, IntegrationEngine, ReportingEngine.
Procurement modules: SupplierManagement and future procurement lifecycle modules.

## Scripts
Use `scripts/task.sh` or `scripts/task.ps1` for setup, ci, test, migrate, seed, reset-db, format, clean, health, dev-api, dev-web.
Scripts must never call Docker.
