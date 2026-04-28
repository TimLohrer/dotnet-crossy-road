using CrossyRoadApi.Config;
using CrossyRoadApi.Database;
using CrossyRoadApi.Models.Game.Map.Elements;
using CrossyRoadApi.Models.Game.Map.Lanes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var appConfig = new AppConfig();
builder.Configuration.Bind(appConfig);
builder.Services.AddSingleton(appConfig);
builder.Services.AddDbContext<CrossyDbContext>(options => options.UseNpgsql(appConfig.Database.ConnectionString).UseSnakeCaseNamingConvention());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UsePathBase("/api/v1");
app.MapControllers();

var lane = new PlainsLane(0);
lane.GenerateElements(666);

Console.WriteLine(lane.ToDto());

app.Run();