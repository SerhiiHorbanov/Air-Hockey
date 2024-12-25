using SFML.System;

static class VectorMath
{
    public static float DistanceSquaredTo(this Vector2f first, Vector2f second)
    {
        Vector2f diff = first - second;

        return (diff.X * diff.X) + (diff.Y + diff.Y);
    }

    public static float Dot(this Vector2f first, Vector2f second)
    {
        return (first.X * second.X) + (first.Y * second.Y);
    }
}