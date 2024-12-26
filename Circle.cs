using SFML.Graphics;
using SFML.System;

class Circle
{
    private readonly int _radius;
    private Vector2f _position;
    public Vector2f Velocity;
    private CircleShape _shape;

    public Circle(int radius, Vector2f position, Vector2f velocity, Color color)
    {
        _radius = radius;
        _position = position;
        Velocity = velocity;

        _shape = new(radius);
        _shape.FillColor = color;
        _shape.Origin = new(radius, radius);
    }

    public void Draw(RenderTarget target)
    {
        _shape.Position = _position;
        _shape.Draw(target, RenderStates.Default);
    }

    public void UpdateVelocity()
    {
        _position += Velocity;
    }

    public void CheckAndResolveCollision(Circle other)
    {
        float distanceSquared = _position.DistanceSquaredTo(other._position);
        float radiusesSum = _radius + other._radius;
        
        if (distanceSquared > radiusesSum * radiusesSum)
            return;
        
        float distance = float.Sqrt(distanceSquared);
        
        Vector2f collisionNormal = (other._position - _position) / distance;

        float overlapDepth = distance - radiusesSum;
        _position += collisionNormal * overlapDepth;
        
        float force = other.Velocity.Dot(collisionNormal) - Velocity.Dot(collisionNormal);
        
        Velocity += collisionNormal * force;
        other.Velocity += collisionNormal * -force;
    }
    
    public void CheckAndResolveCollision(Rectangle rectangle)
    {
        Vector2f closestPoint = rectangle.GetClosestPointTo(_position);
        Console.WriteLine(_position.ToString());
        float distanceSquared = _position.DistanceSquaredTo(closestPoint);
        
        if (distanceSquared > _radius * _radius)
            return;
            
        float distance = float.Sqrt(distanceSquared);
        
        Vector2f collisionNormal = (closestPoint - _position) / distance;
        
        float overlapDepth = distance - _radius;
        _position += collisionNormal * overlapDepth;
        
        float force = -Velocity.Dot(collisionNormal) * 2;
        
        Velocity += collisionNormal * force;
    }
}