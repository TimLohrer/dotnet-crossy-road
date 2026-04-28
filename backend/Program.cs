using System.Numerics;
using CrossyRoadApi.Config;
using CrossyRoadApi.Models.Map;

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

var lane = new CrossyMapLane(CrossyModelPart.PLAINS, Vector3.Zero, CrossyModel.PixelSize, CrossyTheme.Default);
var tree_1 = new CrossyStaticMapElement(CrossyModelPart.TREE_1, lane, Vector3.Zero, CrossyTheme.Default);

Console.WriteLine(lane.Uuid);
Console.WriteLine(lane.GetModelPath());
Console.WriteLine(tree_1.Uuid);
Console.WriteLine(tree_1.GetModelPath());


app.Run();