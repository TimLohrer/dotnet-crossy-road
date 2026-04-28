using System.Numerics;
using CrossyRoadApi.Config;
using CrossyRoadApi.Models.Map;
using CrossyRoadApi.Models.Map.Elements;
using CrossyRoadApi.Models.Map.Lanes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var appConfig = new AppConfig();
builder.Configuration.Bind(appConfig);
builder.Services.AddSingleton(appConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var lane = new PlainsLane(Vector3.Zero);
var stone0 = new Stone0(lane, Vector3.Zero);

Console.WriteLine(lane.Id);
Console.WriteLine(lane.GetModelPath());
Console.WriteLine(lane.GetElements());
Console.WriteLine(stone0.Id);
Console.WriteLine(stone0.GetModelPath());

app.Run();