using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopController(UserManager<CrossyUser> userContext) : ControllerBase
{
    [HttpPost("buy")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrossyUserDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Buy([FromBody] int? skinId)
    {
        if (skinId == null) return BadRequest("Skin does not exist!");
        var user = await userContext.GetUserAsync(User);
        var skin = CrossySkin.FromId((int)skinId);
        if (skin == null) return BadRequest("Skin does not exist!");
        if (user!.Taler < skin.Price) return BadRequest("Not enough Talers");
        if (user.OwnedSkins.Contains(skin.Id)) return BadRequest("Skin already owned");
        user.OwnedSkins.Add(skin.Id);
        user.Skin = skin.Id;
        user.Taler -= skin.Price;

        await userContext.UpdateAsync(user);

        return Ok(user.ToDto());
    }

    [HttpPost("select")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrossyUserDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Select([FromBody] int? skinId)
    {
        if (skinId == null) return BadRequest("Skin does not exist!");
        var user = await userContext.GetUserAsync(User);
        var skin = CrossySkin.FromId((int)skinId);
        if (skin == null) return BadRequest("Skin does not exist!");
        if (!user.OwnedSkins.Contains(skin.Id)) return BadRequest("You don't own this skin!");
        if (user!.Skin == skin.Id) return Ok("Skin already selected");

        user.Skin = skin.Id;
        await userContext.UpdateAsync(user);

        return Ok(user.ToDto());
    }

    [HttpGet("skins")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type =  typeof(List<CrossySkin>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetSkins()
    {
        return Ok(CrossySkin.Skins);
    }
    
}