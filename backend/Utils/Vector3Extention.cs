using System.Numerics;

namespace CrossyRoadApi.Utils;

public static class Vector3Extentions
{
    public static string ToConsoleString(this Vector3 vec)
    {
        return $"({vec.X}, {vec.Y}, {vec.Z})";
    }
}