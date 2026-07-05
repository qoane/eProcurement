using System.Text.Json;
using Lca.EProcurement.Domain;
using DomainApplication = Lca.EProcurement.Domain.Application;
using Lca.EProcurement.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lca.EProcurement.Api.Controllers;

public sealed record ApplicationDesignerDto(string Code, string Name, string Description, string Icon, string Theme, string DefaultLandingPage, string NavigationRoot, List<string> Modules, MetadataStatus Status = MetadataStatus.Draft);

[ApiController, Route("api/applications")]
public sealed class ApplicationsController(EProcurementDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) => Ok(await db.Applications.AsNoTracking().OrderBy(x => x.Code).Select(x => ToDto(x)).ToListAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create(ApplicationDesignerDto dto, CancellationToken ct)
    {
        var app = new DomainApplication(dto.Code, dto.Name, dto.Description, dto.Icon, dto.Theme, dto.DefaultLandingPage, dto.NavigationRoot, JsonSerializer.Serialize(dto.Modules), Status: dto.Status, CreatedBy: "admin");
        db.Applications.Add(app);
        await db.SaveChangesAsync(ct);
        return Created($"/api/applications/{app.Id}", ToDto(app));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, ApplicationDesignerDto dto, CancellationToken ct)
    {
        var app = await db.Applications.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (app is null) return NotFound();
        db.Entry(app).CurrentValues[nameof(DomainApplication.Code)] = dto.Code;
        db.Entry(app).CurrentValues[nameof(DomainApplication.Name)] = dto.Name;
        db.Entry(app).CurrentValues[nameof(DomainApplication.Description)] = dto.Description;
        db.Entry(app).CurrentValues[nameof(DomainApplication.Icon)] = dto.Icon;
        db.Entry(app).CurrentValues[nameof(DomainApplication.Theme)] = dto.Theme;
        db.Entry(app).CurrentValues[nameof(DomainApplication.DefaultLandingPage)] = dto.DefaultLandingPage;
        db.Entry(app).CurrentValues[nameof(DomainApplication.NavigationRoot)] = dto.NavigationRoot;
        db.Entry(app).CurrentValues[nameof(DomainApplication.ModulesJson)] = JsonSerializer.Serialize(dto.Modules);
        db.Entry(app).CurrentValues[nameof(DomainApplication.Status)] = dto.Status;
        db.Entry(app).CurrentValues[nameof(DomainApplication.Modified)] = DateTimeOffset.UtcNow;
        db.Entry(app).CurrentValues[nameof(DomainApplication.ModifiedBy)] = "admin";
        await db.SaveChangesAsync(ct);
        return Ok(ToDto(app with { Code = dto.Code, Name = dto.Name, Description = dto.Description, Icon = dto.Icon, Theme = dto.Theme, DefaultLandingPage = dto.DefaultLandingPage, NavigationRoot = dto.NavigationRoot, ModulesJson = JsonSerializer.Serialize(dto.Modules), Status = dto.Status }));
    }

    [HttpPost("{id:guid}/publish")]
    public Task<IActionResult> Publish(Guid id, CancellationToken ct) => SetStatus(id, MetadataStatus.Active, ct);

    [HttpPost("{id:guid}/archive")]
    public Task<IActionResult> Archive(Guid id, CancellationToken ct) => SetStatus(id, MetadataStatus.Archived, ct);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var app = await db.Applications.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (app is null) return NotFound();
        db.Remove(app);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    async Task<IActionResult> SetStatus(Guid id, MetadataStatus status, CancellationToken ct)
    {
        var app = await db.Applications.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (app is null) return NotFound();
        db.Entry(app).CurrentValues[nameof(DomainApplication.Status)] = status;
        db.Entry(app).CurrentValues[nameof(DomainApplication.Modified)] = DateTimeOffset.UtcNow;
        db.Entry(app).CurrentValues[nameof(DomainApplication.ModifiedBy)] = "admin";
        await db.SaveChangesAsync(ct);
        return Ok(ToDto(app with { Status = status }));
    }

    static object ToDto(DomainApplication app) => new { app.Id, app.Code, app.Name, app.Description, app.Icon, app.Theme, app.DefaultLandingPage, app.NavigationRoot, Modules = JsonSerializer.Deserialize<List<string>>(app.ModulesJson) ?? [], Status = app.Status.ToString(), app.Version, app.Created, app.Modified };
}
