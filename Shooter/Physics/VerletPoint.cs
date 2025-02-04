using OpenTK.Mathematics;
using Shooter.ECS.Components;
using Shooter.Utility;

namespace Shooter.Physics;

public class VerletPoint(float x0, float y0, VerletObject parent)
{
    public readonly VerletObject parent = parent;
    public StructRef<Vector2> position = new((x0, y0));
    public StructRef<Vector2> prevPosition = new((x0,y0));
    public StructRef<Vector2> acceleration = new(Vector2.Zero);
}