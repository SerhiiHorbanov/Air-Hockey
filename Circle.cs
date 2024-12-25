using SFML.Graphics;
using SFML.System;

class Circle
{
    private readonly int _radiusSquared;
    private Vector2f _position;
    private Vector2f _velocity;
    private CircleShape _shape;

    public Circle(int radius, Vector2f position, Vector2f velocity, Color color)
    {
        _radiusSquared = radius * radius;
        _position = position;
        _velocity = velocity;

        _shape = new(radius);
        _shape.FillColor = color;
    }

    public void Draw(RenderTarget target)
    {
        _shape.Position = _position;
        _shape.Draw(target, RenderStates.Default);
    }

    public void Update(params Circle[] circlesToCheckCollisions)
    {
        foreach (Circle each in circlesToCheckCollisions)
            CheckAndResolveCollision(each);
        
        _position += _velocity;
    }

    private void CheckAndResolveCollision(Circle other)
    {   
        if (ReferenceEquals(other, this))
            return;

        float distanceSquared = _position.DistanceSquaredTo(other._position);
        
        if (distanceSquared > _radiusSquared + other._radiusSquared)
            return;
        
        float distance = float.Sqrt(distanceSquared);
        
        Vector2f collisionNormal = (_position - other._position) / distance;
        float force = _velocity.Dot(collisionNormal) - other._velocity.Dot(collisionNormal);

        _velocity += collisionNormal * force;
    }
}