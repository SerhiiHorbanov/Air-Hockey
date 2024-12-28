using SFML.Graphics;
using SFML.System;

class Circle
{
    private readonly int _radius;
    public Vector2f Position;
    public Vector2f Velocity;
    private CircleShape _shape;

    public Circle(int radius, Vector2f position, Vector2f velocity, Color color)
    {
        _radius = radius;
        Position = position;
        Velocity = velocity;

        _shape = new(radius);
        _shape.FillColor = color;
        _shape.Origin = new(radius, radius);
    }

    public void Draw(RenderTarget target)
    {
        _shape.Position = Position;
        _shape.Draw(target, RenderStates.Default);
    }

    public void UpdateVelocity()
    {
        Position += Velocity;
    }

    public void CheckAndResolveCollision(Circle other)
    {
        float distanceSquaredToCenter = Position.DistanceSquaredTo(other.Position);
        float radiusesSum = _radius + other._radius;
        
        if (distanceSquaredToCenter > radiusesSum * radiusesSum)
            return;
        
        float distanceToCenter = float.Sqrt(distanceSquaredToCenter);

        Vector2f collisionPoint = (Position - other.Position) / distanceToCenter;
        collisionPoint *= other._radius;
        collisionPoint += other.Position;
        float distanceToCollisionPoint = distanceToCenter - other._radius;
        
        ResolveCollisionWithPoint(collisionPoint, other.Velocity, distanceToCollisionPoint, 1.5f);
    }

    private void ResolveCollisionWithPoint(Vector2f point, Vector2f velocity, float distance, float bounce = 1)
    {
        if (distance <= 0)
            return;
        
        Vector2f collisionNormal = (point - Position) / distance;

        float overlapDepth = distance - _radius;
        Position += collisionNormal * overlapDepth;

        float force = velocity.Dot(collisionNormal) - Velocity.Dot(collisionNormal);
        
        Velocity += collisionNormal * force * bounce;
    }

    private void ResolveCollisionWithPoint(Vector2f point, float distance, float bounce = 1)
        => ResolveCollisionWithPoint(point, new(0, 0), distance, bounce);
    
    public void CheckAndResolveCollision(Rectangle rectangle)
    {
        Vector2f closestPoint = rectangle.GetClosestPointTo(Position);
        float distanceSquared = Position.DistanceSquaredTo(closestPoint);
        
        if (distanceSquared > _radius * _radius)
            return;

        if (distanceSquared <= 0)
        {
            Vector2f unclippingDirection = rectangle.GetClosestPointOnPerimeter(Position);
            float unclippingLength = Position.DistanceTo(unclippingDirection);
            unclippingDirection -= Position;
            unclippingDirection /= unclippingLength;
            unclippingLength += _radius;

            Position += unclippingDirection * unclippingLength;
        }
            
        float distance = float.Sqrt(distanceSquared);
        ResolveCollisionWithPoint(closestPoint, distance, 2);
    }
}