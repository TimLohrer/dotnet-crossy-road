namespace CrossyRoadApi.Models.Database;

public class CrossySkin(int id, string name, int price, CrossySkin.SkinRarity rarity, string modelName)
{
    public enum SkinRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public static readonly CrossySkin Chicken = new(0, "Chicken", 0, SkinRarity.Common, "chicken");

    public static readonly CrossySkin Duck = new(1, "Duck", 100, SkinRarity.Uncommon, "chicken");

    public static readonly List<CrossySkin> Skins = [Chicken, Duck];
    
    public readonly int Id = id;
    public readonly string ModelName = modelName;
    public readonly string Name = name;
    public readonly int Price = price;
    public readonly SkinRarity Rarity = rarity;

    public static CrossySkin? FromId(int id)
    {
        if (id == Chicken.Id) return Chicken;
        if (id == Duck.Id) return Duck;
        return null;
    }
}