using Application.Contracts;
using Application.Entries;
using Application.Foods;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/nutrition/entries")]
public class EntriesController : ControllerBase
{
    private readonly IMediator _med;
    public EntriesController(IMediator med) => _med = med;

    [HttpPost("food")]
    public async Task<ActionResult<NutritionEntryDto>> LogFood([FromBody] LogFoodEntryCommand cmd, CancellationToken ct)
        => Ok(await _med.Send(cmd, ct));

    [HttpPost("quick-add")]
    public async Task<ActionResult<NutritionEntryDto>> QuickAdd([FromBody] QuickAddEntryCommand cmd, CancellationToken ct)
        => Ok(await _med.Send(cmd, ct));

    [HttpGet("daily")]
    public async Task<ActionResult<IReadOnlyList<NutritionEntryDto>>> GetDaily([FromQuery] DateOnly date, CancellationToken ct)
        => Ok(await _med.Send(new GetDailyEntriesQuery(date), ct));

    [HttpGet("daily/summary")]
    public async Task<ActionResult<DailySummaryDto>> GetDailySummary([FromQuery] DateOnly date, CancellationToken ct)
        => Ok(await _med.Send(new GetDailySummaryQuery(date), ct));
}