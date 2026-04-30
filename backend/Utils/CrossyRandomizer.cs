namespace CrossyRoadApi.Utils;

public static class CrossyRandomizer
{
    public static Random Get(int seed, int zPosition)
    {
        var zRandom = new Random(zPosition);
        var zRandomizer1 = zRandom.Next(1, 99);
        var zRandomizer2 = zRandom.Next(1, 99);
        var zBasedSeed = zRandomizer1 < seed ? seed / zRandomizer1 * zRandomizer2 : seed * zRandomizer2 + zRandomizer2;

        return new Random(zBasedSeed);
    }
}