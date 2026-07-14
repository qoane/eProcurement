# security checklist

ProcuraFlow Phase 1 operational readiness guidance for enterprise demo and future production deployment.

## Required configuration
- ConnectionStrings:Default for SQL Server.
- Jwt:Issuer, Jwt:Audience and Jwt:SigningKey from secret storage.
- CORS/allowed origins for the frontend URL.
- Operations:PublicBaseUrl and Operations:FrontendBaseUrl.
- SMTP/SMS settings only when those channels are enabled.
- Documents:RootPath when document storage is enabled.
- Integration endpoints only for enabled systems.

## Deployment notes
- Build backend with dotnet publish; apply EF Core migrations before start.
- Build frontend with npm run build and host static assets over HTTPS.
- IIS or Windows service hosting is acceptable for the API; require HTTPS in production.
- Do not store real secrets in source control.

## Backup and restore
Phase 1 records backup readiness evidence only. Configure real database backups in SQL Server/infrastructure tooling and document restore drills, retention, encryption and off-site storage.

## Monitoring and troubleshooting
Use /health/live, /health/ready, /health/database, /health/storage and /health/integrations. Use correlation IDs from API errors to find structured logs and audit events. Review /app/operations for slow requests, recent errors, backup evidence and configuration validation.
