using System.Numerics;
using CrossyRoadApi.Config;
using CrossyRoadApi.Database;
using CrossyRoadApi.Models.Game.Map.Elements;
using CrossyRoadApi.Models.Game.Map.Lanes;
using CrossyRoadApi.Utils;
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
var stone0 = new Stone0(lane, 3);

Console.WriteLine(lane.Id);
Console.WriteLine(lane.GetModelPath());
Console.WriteLine(lane.GetElements());

Console.WriteLine(stone0.Id);
Console.WriteLine(stone0.GetModelPath());
stone0.GetPositions().ForEach(v => Console.WriteLine(v.ToConsoleString()));

app.Run();