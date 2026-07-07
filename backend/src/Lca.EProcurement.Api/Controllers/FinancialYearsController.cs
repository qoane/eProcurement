using Lca.EProcurement.Application;
using Microsoft.AspNetCore.Mvc;

namespace Lca.EProcurement.Api.Controllers;

[ApiController]
[Route("api/financial-years")]
public sealed class FinancialYearsController(IFinancialYearApplicationService financialYears) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await financialYears.GetAsync(ct));
}
