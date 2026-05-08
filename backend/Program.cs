using System.Security.Claims;
using CrossyRoadApi.Config;
using CrossyRoadApi.Controllers;
using CrossyRoadApi.Database;
using CrossyRoadApi.Models.Database;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.IncludeFields = true; });
builder.Services.AddOpenApi();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
});

var appConfig = new AppConfig();
builder.Configuration.Bind(appConfig);
builder.Services.AddSingleton(appConfig);
builder.Services.AddDbContext<CrossyDbContext>(options =>
    options.UseNpgsql(appConfig.Database.ConnectionString).UseSnakeCaseNamingConvention());
builder.Services.AddSignalR().AddJsonProtocol(options => { options.PayloadSerializerOptions.IncludeFields = true; });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, opt =>
    {
        opt.Cookie.Name = "CrossyRoad.Auth";

        opt.Cookie.IsEssential = true;
        opt.ExpireTimeSpan = TimeSpan.FromHours(10);

        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Cookie.Domain = appConfig.Domain;

        opt.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        opt.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    })
    .AddOpenIdConnect("bosch", "Bosch", opt =>
    {
        opt.MetadataAddress = appConfig.BoschOAuth.MetaDataAddress;
        opt.GetClaimsFromUserInfoEndpoint = true;
        opt.ClientId = appConfig.BoschOAuth.ClientId;
        opt.ClientSecret = appConfig.BoschOAuth.ClientSecret;
        opt.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
        opt.SignInScheme = IdentityConstants.ExternalScheme;
        opt.CallbackPath = "/auth/signin-oidc-bosch";
        opt.ResponseType = "id_token token";
        opt.SaveTokens = true;
        foreach (var scope in appConfig.BoschOAuth.Scopes.Split(",").Select(x => x.Trim()))
            opt.Scope.Add(scope);
    }).AddOpenIdConnect("microsoft", "Microsoft", opt =>
    {
        opt.MetadataAddress = appConfig.MicrosoftOAuth.MetaDataAddress;
        opt.GetClaimsFromUserInfoEndpoint = true;
        opt.ClientId = appConfig.MicrosoftOAuth.ClientId;
        opt.ClientSecret = appConfig.MicrosoftOAuth.ClientSecret;
        opt.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
        opt.SignInScheme = IdentityConstants.ExternalScheme;
        opt.CallbackPath = "/auth/signin-oidc-microsoft";
        opt.ResponseType = OpenIdConnectResponseType.Code;
        opt.UsePkce = true;
        opt.SaveTokens = true;
        foreach (var scope in appConfig.MicrosoftOAuth.Scopes.Split(",").Select(x => x.Trim()))
            opt.Scope.Add(scope);
    }).AddCookie(IdentityConstants.ExternalScheme, opt => { opt.Cookie.Name = "Manager.External"; });

var ib = builder.Services.AddIdentityCore<CrossyUser>(opt =>
    {
        opt.SignIn.RequireConfirmedAccount = false;
        opt.User.RequireUniqueEmail = true;
        opt.User.AllowedUserNameCharacters =
            "@abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 6;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;

        opt.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
    }).AddEntityFrameworkStores<CrossyDbContext>()
    .AddRoles<UserRole>()
    .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<CrossyUser, UserRole>>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

builder.Services.AddScoped<IRoleStore<UserRole>, RoleStore<UserRole, CrossyDbContext, Guid>>();
builder.Services.AddScoped<IUserStore<CrossyUser>, UserStore<CrossyUser, UserRole, CrossyDbContext, Guid>>();

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CrossyDbContext>();
    Console.WriteLine("Applying database migrations...");
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseForwardedHeaders();

app.UsePathBase("/api/v1");
app.UseRouting();

// CORS only matters cross-origin (the dev frontend on :5173). In production the
// frontend is served on the same origin via the reverse proxy, so it's a no-op there.
if (app.Environment.IsDevelopment()) app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

app.MapHub<GameHub>("/game/ws");

app.Run();