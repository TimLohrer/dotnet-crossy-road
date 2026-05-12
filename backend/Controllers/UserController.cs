using CrossyRoadApi.Dto;
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

    [HttpPatch("@me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrossyUserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUsername([FromBody] PatchUserDto patchedUser)
    {
        var user = await userContext.GetUserAsync(User);
        if (user == null) return Unauthorized();
        if (patchedUser.Username == null || user.UserName! == patchedUser.Username.ToLower()) return Ok(user.ToDto());

        var allowedChars = "@abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        user.UserName = new string(patchedUser.Username.ToLower().Where(x => allowedChars.Contains(x)).ToArray());
        user.UserName = user.UserName.Length > 50 ? user.UserName.Substring(0, 50) : user.UserName;

        var existingUser = await userContext.FindByNameAsync(user.UserName.ToLower());
        if (existingUser != null && existingUser.Id != user.Id) return BadRequest("Username is already taken!");

        var result = await userContext.UpdateAsync(user);
        if (!result.Succeeded) return StatusCode(500, result.Errors);

        return Ok(user.ToDto());
    }
}