# Remaining Functional Gaps

Date: 2026-07-15

- Empty SQL Server migration and seed execution were not verified in this repair container because no SQL Server instance is available. Run `dotnet run --project backend/src/Lca.EProcurement.Api/Lca.EProcurement.Api.csproj -- --seed` against a reachable SQL Server database to validate the full migration and seed path.
- Payment record tracking was not expanded during this stabilization pass. The repair only ensured missing payment-related authorization policies are registered for already-referenced endpoints.
