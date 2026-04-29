using CrossyRoadApi.Config;
using CrossyRoadApi.Controllers;
using CrossyRoadApi.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.IncludeFields = true; });
builder.Services.AddOpenApi();
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
builder.Services.AddSignalR().AddJsonProtocol();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowFrontend");

app.UseWebSockets();

app.UsePathBase("/api/v1");
app.MapControllers();

app.MapHub<GameHub>("/game/ws");

app.Run();