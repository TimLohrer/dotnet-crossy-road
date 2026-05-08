using System.Numerics;
using System.Runtime.Serialization;
using Microsoft.OpenApi;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyModel
{
    protected CrossyModel(CrossyModelPart modelPart, Vector3 position)
    {
        Id = Guid.NewGuid();
        Position = position;
        ModelPart = modelPart;
        Theme = CrossyTheme.Default;
    }

    public Guid Id { get; }
    public CrossyModelPart ModelPart { get; }
    public Vector3 Position { get; protected set; }
    public CrossyTheme Theme { get; protected set; }

    public abstract string GetModelPath();

    protected string GetModelPath(string modelType)
    {
        return
            $"/models/{Theme.ToString().ToLower()}/{modelType}/{ModelPart.GetAttributeOfType<EnumMemberAttribute>()!.Value}.gltf";
    }

    public abstract void SetPosition(Vector3 newPosition);

    public void SetTheme(CrossyTheme theme)
    {
        Theme = theme;
    }
}