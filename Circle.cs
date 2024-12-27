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
        float distanceSquaredToCenter = _position.DistanceSquaredTo(other._position);
        float radiusesSum = _radius + other._radius;
        
        if (distanceSquaredToCenter > radiusesSum * radiusesSum)
            return;
        
        float distanceToCenter = float.Sqrt(distanceSquaredToCenter);

        Vector2f collisionPoint = (_position - other._position) / distanceToCenter;
        collisionPoint *= other._radius;
        collisionPoint += other._position;
        float distanceToCollisionPoint = distanceToCenter - other._radius;
        
        ResolveCollisionWithPoint(collisionPoint, other.Velocity, distanceToCollisionPoint);
    }

    private void ResolveCollisionWithPoint(Vector2f point, Vector2f velocity, float distance)
    {
        Vector2f collisionNormal = (point - _position) / distance;

        float overlapDepth = distance - _radius;
        _position += collisionNormal * overlapDepth;

        float force = velocity.Dot(collisionNormal) - Velocity.Dot(collisionNormal);
        
        Velocity += collisionNormal * force;
    }

    private void ResolveCollisionWithPoint(Vector2f point, float distance)
        => ResolveCollisionWithPoint(point, new(0, 0), distance);
    
    public void CheckAndResolveCollision(Rectangle rectangle)
    {
        Vector2f closestPoint = rectangle.GetClosestPointTo(_position);
        float distanceSquared = _position.DistanceSquaredTo(closestPoint);
        
        if (distanceSquared > _radius * _radius)
            return;

        float distance = float.Sqrt(distanceSquared);
        ResolveCollisionWithPoint(closestPoint, distance);
    }
}