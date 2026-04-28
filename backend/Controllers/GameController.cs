using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Game.Map.Lanes;
using Microsoft.AspNetCore.Mvc;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    [HttpGet("{z}")]
    public CrossyMapLaneDto Get([FromRoute] int z, [FromQuery] int seed)
    {
        var lane = new PlainsLane(int.Max(0, z));
        lane.GenerateElements(seed);
        return lane.ToDto();
    }
}