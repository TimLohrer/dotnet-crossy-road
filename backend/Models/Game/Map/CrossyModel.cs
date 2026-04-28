using System.ComponentModel;
using System.Numerics;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyModel
{
    public Guid Id { get; }
    public CrossyModelPart ModelPart { get; }
    public Vector3 Position { get; protected set; }
    public CrossyTheme Theme { get; protected set; }

    // Size of one pixel in a model (constant is used to provide position offsets)
    public static readonly float PixelSize = 0.05f;
    
    protected CrossyModel(CrossyModelPart modelPart, Vector3 position)
    {
        Id = Guid.NewGuid();
        Position = position;
        ModelPart = modelPart;
        Theme = CrossyTheme.Default;
    }

    public abstract string GetModelPath();
    protected string GetModelPath(string modelType) => $"/models/{Theme.ToString().ToLower()}/{modelType}/{ModelPart.GetAttributeOfType<JsonPropertyNameAttribute>()!.Name}.gltf";

    public abstract void SetPosition(int position);
    public void SetTheme(CrossyTheme theme) => Theme = theme;
}