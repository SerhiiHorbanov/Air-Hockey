using SFML.Graphics;
using SFML.System;

class Rectangle
{
    private FloatRect _rect;
    private RectangleShape _shape;

    public Rectangle(FloatRect rect, Color color)
    {
        _rect = rect;
        _shape = new(_rect.Size);
        _shape.FillColor = color;
    }

    public void Draw(RenderTarget target)
    {
        _shape.Position = _rect.Position;
        _shape.Size = _rect.Size;
        _shape.Draw(target, RenderStates.Default);
    }

    public Vector2f GetClosestPointTo(Vector2f position)
        => VectorMath.Clamp(position, _rect);
}