using SFML.Graphics;
using SFML.System;

class Rectangle
{
    private FloatRect _rect;
    private RectangleShape _shape;

    public float Left
        => _rect.Left;
    public float Right
        => _rect.Left + _rect.Width;
    public float Top
        => _rect.Top;
    public float Bottom
        => _rect.Top + _rect.Height;
    
    public Rectangle(FloatRect rect, Color color)
    {
        _rect = rect;
        _shape = new(_rect.Size);
        _shape.FillColor = color;
    }

    public Rectangle(Vector2f leftTop, Vector2f rectangleSize, Color color) : 
        this(new FloatRect(leftTop.X, leftTop.Y, rectangleSize.X, rectangleSize.Y), color)
    { }

    public void Draw(RenderTarget target)
    {
        _shape.Position = _rect.Position;
        _shape.Size = _rect.Size;
        _shape.Draw(target, RenderStates.Default);
    }

    public Vector2f GetClosestPointTo(Vector2f position)
        => VectorMath.Clamp(position, _rect);

    bool IsXInside(float x)
        => x > Left && x < Right;
    bool IsYInside(float y)
        => y > Top && y < Bottom;
    bool IsInside(Vector2f point)
        => IsXInside(point.X) && IsYInside(point.Y);
    
    public Vector2f GetClosestPointOnPerimeter(Vector2f position)
    {
        if (!IsInside(position))
            return GetClosestPointTo(position);
        
        Vector2f closestOnLeft = new(Left, position.Y);
        Vector2f closestOnRight = new(Right, position.Y);
        Vector2f closestOnTop = new(position.X, Top);
        Vector2f closestOnBottom = new(position.X, Bottom);
        
        return position.GetClosestPoint(closestOnLeft, closestOnRight, closestOnTop, closestOnBottom);
    }
}