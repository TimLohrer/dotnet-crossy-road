using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LeaderboardController(UserManager<CrossyUser> userManager) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CrossyUserMinimalDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CrossyUserMinimalDto>>> GetLeaderboard()
    {
        var top10 = await userManager.Users.OrderByDescending(u => u.HighScore).Take(10).ToListAsync();
        return Ok(top10.Select(u => u.ToMinimalDto()).ToList());
    }
}