using System.Security.Claims;
using CrossyRoadApi.Config;
using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace CrossyRoadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    SignInManager<CrossyUser> signInManager,
    UserManager<CrossyUser> userManager,
    AppConfig config,
    ILogger<AuthController> logger)
    : ControllerBase
{
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

        var claims = info.Principal.Claims.ToList();

        var providerKey = info.ProviderKey;
        if (info.LoginProvider == "microsoft")
            providerKey = claims.FirstOrDefault(x => x.Type == ClaimConstants.ObjectId)?.Value;

        var userNameClaim = claims.SingleOrDefault(x => x.Type == ClaimConstants.Name)?.Value;

        if (string.IsNullOrEmpty(providerKey) || string.IsNullOrEmpty(userNameClaim)) return UnprocessableEntity();

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

            // externalUser.UserName = userNameClaim;

            await userManager.UpdateAsync(externalUser);

            await signInManager.SignOutAsync();
            await signInManager.SignInAsync(externalUser, true, info.LoginProvider);

            // Success
            return returnUrl != null ? Redirect(returnUrl) : NoContent();
        }

        if (result.IsLockedOut) return BadRequest("Locked Out");

        // Check if we are already signed in (should never be the case)
        if (User.Identity is { IsAuthenticated: true })
            return returnUrl != null
                ? Redirect(
                    $"/api/v1/signout?redirectUrl={Base64UrlEncoder.Encode($"/api/v1/signin/{info.ProviderKey}?returnUrl={returnUrl}")}")
                : BadRequest("You are already authenticated??");

        var user = new CrossyUser
        {
            Email = claims.Single(x => x.Type == ClaimTypes.Email).Value,
            // UserName = userNameClaim
            UserName = $"Player_{new Random().Next(100000, 999999)}"
        };

        var userCreateResult = await userManager.CreateAsync(user);
        if (!userCreateResult.Succeeded) return BadRequest("Failed to create user :(");

        var addedLoginResult = await userManager.AddLoginAsync(user,
            new UserLoginInfo(info.LoginProvider, providerKey, info.ProviderDisplayName));
        if (!addedLoginResult.Succeeded) return BadRequest("Failed to create login credentials :(");

        await signInManager.SignInAsync(user, true, info.LoginProvider);
        return returnUrl != null ? Redirect(returnUrl) : NoContent();
    }
}