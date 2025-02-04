using OpenTK.Mathematics;
using Shooter.Utility;

namespace Shooter.Physics;

public struct CollisionData(float depth, Vector2 normal, EdgeConstraint hitEdge, VerletPoint hitVert)
{
    public float Depth { get; set; } = depth;
    public Vector2 Normal { get; set; } = normal;
    public EdgeConstraint HitEdge { get; set; } = hitEdge;
    public VerletPoint HitVert = hitVert;
}