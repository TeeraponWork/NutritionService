using Application.Contracts;
using Application.Foods;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/nutrition/foods")]
public class FoodsController : ControllerBase
{
    private readonly IMediator _med;
    public FoodsController(IMediator med) => _med = med;

    [HttpPost]
    public async Task<ActionResult<FoodDto>> Create([FromBody] CreateFoodCommand cmd, CancellationToken ct)
        => Ok(await _med.Send(cmd, ct));
}