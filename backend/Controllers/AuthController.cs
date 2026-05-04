using System.Security.Claims;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    SignInManager<CrossyPlayer> signInManager,
    UserManager<CrossyPlayer> userManager,
    ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpGet("@me")]
    [Authorize]
    [ProducesResponseType(typeof(CrossyPlayerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userManager.GetUserAsync(User);

        if (user == null) return Unauthorized();

        return Ok(user.ToDto());
    }
    
    [HttpGet("signin/{providerName}")]
    public IActionResult Login(string providerName, string? returnUrl = null)
    {
        var redirectUrl = Url.Action("ExternalCallback", new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);
        return new ChallengeResult(providerName, properties);
    }

    [Authorize]
    [HttpGet("signout")]
    public async Task<IActionResult> SignOut(string? returnUrl = null)
    {
        await signInManager.SignOutAsync();

        return returnUrl != null ? Redirect(returnUrl) : NoContent();
    }
    
    [AllowAnonymous]
    [HttpGet("external/callback")]
    public async Task<IActionResult> ExternalCallback(string? returnUrl = null)
    {
        if (User.Identity is { IsAuthenticated: true }) await signInManager.SignOutAsync();

        ExternalLoginInfo? info;
        try
        {
            info = await signInManager.GetExternalLoginInfoAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting external login info");
            return StatusCode(500);
        }
        
        if (info == null)
        {
            logger.LogInformation("Info is null");
            return BadRequest();
        }

        var providerKey = info.ProviderKey;
        if (info.LoginProvider == "bosch")
            providerKey = info.Principal.Claims.Single(x => x.Type == ClaimConstants.ObjectId).Value;

        var claims = info.Principal.Claims.ToList();

        var idClaim = claims.SingleOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
        var displayNameClaim = claims.SingleOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;

        if (string.IsNullOrEmpty(idClaim) ||
            string.IsNullOrEmpty(displayNameClaim)) return UnprocessableEntity();

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, providerKey, true, true);
        if (result.Succeeded)
        {
            var externalUser = await userManager.FindByLoginAsync(info.LoginProvider, providerKey);

            if (externalUser == null) return StatusCode(500);

            var accessToken = info.AuthenticationTokens?.SingleOrDefault(x => x.Name == "access_token")?.Value;
            if (accessToken != null)
            {
                logger.LogInformation("Access token set");
                await userManager.SetAuthenticationTokenAsync(externalUser, info.LoginProvider, "access_token",
                    accessToken);
            }
            else
            {
                logger.LogInformation("Access token not set");
            }
            
            externalUser.Username = displayNameClaim;

            await userManager.UpdateAsync(externalUser);

            await signInManager.SignOutAsync();
            await signInManager.SignInAsync(externalUser, true, info.LoginProvider);

            // Success
            return returnUrl != null ? Redirect(returnUrl) : NoContent();
        }

        if (result.IsLockedOut) return BadRequest("Locked Out");

        // Check if we are already signed in (should never be the case)
        if (User.Identity is { IsAuthenticated: true })
        {
            return BadRequest();
        }

        var user = new CrossyPlayer
        {
            Id =  Guid.Parse(idClaim),
            Username = displayNameClaim
        };

        var userCreateResult = await userManager.CreateAsync(user);
        if (!userCreateResult.Succeeded) return BadRequest();

        var addedLoginResult = await userManager.AddLoginAsync(user,
            new UserLoginInfo(info.LoginProvider, providerKey, info.ProviderDisplayName));
        if (!addedLoginResult.Succeeded) return BadRequest();

        await signInManager.SignInAsync(user, true, info.LoginProvider);
        return returnUrl != null ? Redirect(returnUrl) : NoContent();
    }
}