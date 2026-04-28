using System.ComponentModel;
using System.Numerics;
using Microsoft.OpenApi;

namespace CrossyRoadApi.Models.Map;

public abstract class CrossyModel
{
    public Guid Uuid { get; }
    public CrossyModelPart ModelPart { get; }
    public Vector3 Position { get; protected set; }
    public CrossyTheme Theme { get; protected set; }

    // Size of one pixel in a model (constant is used to provide position offsets)
    public static readonly float PixelSize = 0.05f;
    
    protected CrossyModel(CrossyModelPart modelPart, Vector3 position, CrossyTheme? theme)
    {
        Uuid = Guid.NewGuid();
        Position = position;
        ModelPart = modelPart;
        Theme = theme.GetValueOrDefault(CrossyTheme.Default);
    }

    public abstract string GetModelPath();
    protected string GetModelPath(string modelType) => $"/models/{Theme.ToString().ToLower()}/{modelType}/{ModelPart.GetAttributeOfType<DescriptionAttribute>().Description}.gltf";

    public void SetPosition(Vector3 position) => Position = position;
    public void SetTheme(CrossyTheme theme) => Theme = theme;
}