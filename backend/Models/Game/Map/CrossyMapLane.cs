using System.Numerics;
using CrossyRoadApi.Dto;
using CrossyRoadApi.Models.Game.Map.Elements;
using CrossyRoadApi.Utils;

namespace CrossyRoadApi.Models.Game.Map;

public abstract class CrossyMapLane : CrossyModel
{
    protected static int LaneLength = 25;
    public List<CrossyMapElement> Elements = [];

    public CrossyMapLane(CrossyModelPart modelPart, int seed, int zPosition, bool empty = false) : base(modelPart,
        new Vector3(0, 0, zPosition))
    {
        // Generate Map
        if (!empty)
        {
            Randomizer = CrossyRandomizer.Get(seed, zPosition);
            GenerateElements();
        }
    }

    protected Random Randomizer { get; }
    public abstract CrossyModelPart LaneType { get; }

    protected abstract void GenerateElements();

    public override string GetModelPath()
    {
        return GetModelPath("lanes");
    }

    public override void SetPosition(Vector3 newPosition)
    {
        Position = newPosition;
        foreach (var element in Elements)
        {
            var elementPos = element.Position;
            element.SetPosition(new Vector3(elementPos.X, elementPos.Y, newPosition.Z));
        }
    }

    public void GenerateTalers(float yPos)
    {
        var hasTalers = Randomizer.NextDouble() < 0.075;
        if (!hasTalers) return;
        var talerCount = Randomizer.Next(1, 3);
        for (var i = 0; i < talerCount; i++)
        {
            int talerX;
            do
            {
                talerX = Randomizer.Next(-6, 7);
            } while (Elements.Any(e => e.Position.X == talerX));

            var taler = new Taler(this, talerX);
            taler.SetPosition(new Vector3(talerX, yPos, 0));
            Elements.Add(taler);
        }
    }

    // lane is 25 * 16 long, one field is 16x16 -> idk field now kinda cursed length? it works tough
    public int GetMapPositionFromLanePositionIndex(int lanePositionIndex)
    {
        return lanePositionIndex - 12;
    }

    public List<CrossyMapElement> GetElements()
    {
        return Elements;
    }

    public void AddElement(CrossyMapElement element)
    {
        Elements.Add(element);
    }

    public void RemoveElementById(Guid elementId)
    {
        Elements.RemoveAll(e => e.Id == elementId);
    }

    public void ClearElements()
    {
        Elements.Clear();
    }

    public CrossyMapLaneDto ToDto()
    {
        return new CrossyMapLaneDto
        {
            Id = Id,
            Type = ModelPart,
            Position = Position,
            ModelLocation = GetModelPath(),
            Elements = Elements.Select(e => e.ToDto()).ToList()
        };
    }
}