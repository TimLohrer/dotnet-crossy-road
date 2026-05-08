namespace CrossyRoadApi.Models.Database;

public class CrossySkin(int id, string name, int price, CrossySkin.SkinRarity rarity, string modelName)
{
    public enum SkinRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public static readonly CrossySkin Chicken = new(0, "Chicken", 0, SkinRarity.Common, "chicken");

    public static readonly CrossySkin Chick = new(1, "Chick", 50, SkinRarity.Rare, "chick");
    public static readonly CrossySkin Rabbit = new(2, "Rabbit", 50, SkinRarity.Rare, "rabbit");
    public static readonly CrossySkin Monkey = new(3, "Monkey", 200, SkinRarity.Epic, "monkey");
    public static readonly CrossySkin Snail = new(4, "Snail", 200, SkinRarity.Epic, "snail");
    public static readonly CrossySkin Octopus = new(5, "Octopus", 500, SkinRarity.Legendary, "octopus");

    public static readonly List<CrossySkin> Skins = [Chicken, Chick, Rabbit, Monkey, Snail, Octopus];
    
    public readonly int Id = id;
    public readonly string ModelName = modelName;
    public readonly string Name = name;
    public readonly int Price = price;
    public readonly SkinRarity Rarity = rarity;

    public static CrossySkin? FromId(int id)
    {
        if (id == Chicken.Id) return Chicken;
        if (id == Chick.Id) return Chick;
        if (id == Rabbit.Id) return Rabbit;
        if (id == Monkey.Id) return Monkey;
        if (id == Snail.Id) return Snail;
        if (id == Octopus.Id) return Octopus;
        return null;
    }
}