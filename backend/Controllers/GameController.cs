using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Game.Map;
using Microsoft.AspNetCore.Mvc;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    [HttpGet("{z}")]
    public List<CrossyMapLaneDto> Get([FromRoute] int z, [FromQuery] int seed)
    {
        var lanes = CrossyMapGenerator.GenerateMapSection(seed, int.Max(0, z));
        return lanes.Select(l => l.ToDto()).ToList();
    }
}