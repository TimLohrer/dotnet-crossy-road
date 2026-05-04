using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

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
        var userId = User.GetObjectId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get user id???");
        }
        
        var user = await userContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        
        return Ok(user);
    }
}