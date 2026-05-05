using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserManager<CrossyUser> userContext) : ControllerBase
{
    [HttpGet("@me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        var user = await userContext.GetUserAsync(User);
        if (user == null) return Unauthorized();
        return Ok(user.ToDto());
    }
}