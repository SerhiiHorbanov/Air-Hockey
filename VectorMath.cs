using SFML.Graphics;
using SFML.System;

static class VectorMath
{
    public static float DistanceSquaredTo(this Vector2f first, Vector2f second)
    {
        Vector2f diff = first - second;

        return (diff.X * diff.X) + (diff.Y * diff.Y);
    }

    public static float Dot(this Vector2f first, Vector2f second)
    {
        return (first.X * second.X) + (first.Y * second.Y);
    }

    public static Vector2f Clamp(Vector2f vector, FloatRect rect)
    {
        if (vector.X < rect.Left)
            vector.X = rect.Left;
        else if (vector.X > rect.Left + rect.Width)
            vector.X = rect.Left + rect.Width;
        
        if (vector.Y < rect.Top)
            vector.Y = rect.Top;
        else if (vector.Y > rect.Top + rect.Height)
            vector.Y = rect.Top + rect.Height;
        return vector;
    }
}