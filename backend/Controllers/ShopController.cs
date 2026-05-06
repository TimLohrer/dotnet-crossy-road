using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopController(UserManager<CrossyUser> userContext) : ControllerBase
{
    [HttpGet("buy")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Buy(CrossySkin skin)
    {
        var user = await userContext.GetUserAsync(User);
        if (user!.Taler < 100) return BadRequest("Not enough Talers");
        if (user.OwnedSkins.Contains(skin)) return BadRequest("Skin already owned");
        user.OwnedSkins.Add(skin);
        user.Skin = skin;
        user.Taler -= 100;

        await userContext.UpdateAsync(user);

        return Ok();
    }

    [HttpGet("select")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Select(CrossySkin skin)
    {
        var user = await userContext.GetUserAsync(User);
        
        if (user!.OwnedSkins.Contains(skin)) return BadRequest("Skin already owned");
        user.Skin = skin;
        
        await userContext.UpdateAsync(user);
        return Ok();
    }
}