using OpenTK.Mathematics;
using Shooter.ECS.Components;
using Shooter.Utility;

namespace Shooter.Physics;

public class EdgeConstraint(VerletObject parent, StructRef<Vector2> start, StructRef<Vector2> end)
{
    public VerletObject parent = parent;
    public StructRef<Vector2> start = start;
    public StructRef<Vector2> end = end;

    public readonly float desiredLength = Vector2.Distance(start.Value, end.Value);
}