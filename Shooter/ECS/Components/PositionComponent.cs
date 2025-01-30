using OpenTK.Mathematics;

namespace Shooter.ECS.Components;

public class PositionComponent(float x = 0, float y = 0) : IComponent
{
    public float x = x;
    public float y = y;

    public Vector2 XY => new(this.x, this.y);
}