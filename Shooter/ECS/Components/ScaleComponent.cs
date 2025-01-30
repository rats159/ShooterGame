using System.Data.SqlTypes;
using OpenTK.Mathematics;

namespace Shooter.ECS.Components;

public class ScaleComponent(float sx = 1, float sy = 1) : IComponent
{
    public float x = sx;
    public float y = sy;
    
    public Vector2 XY => new(this.x, this.y);
}