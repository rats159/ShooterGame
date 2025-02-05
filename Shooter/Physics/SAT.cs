using OpenTK.Mathematics;
using Shooter.ECS;
using Shooter.ECS.Components;

namespace Shooter.Physics;

public static class SAT
{
    public static bool IsColliding(Entity a, Entity b, out CollisionData? data)
    {
        Transform aTransform = a.Get<Transform>();
        Transform bTransform = b.Get<Transform>();
        VerletObject aCollider = a.Get<VerletObject>();
        VerletObject bCollider = b.Get<VerletObject>();

        Span<Vector2> aVertices = stackalloc Vector2[4];
        Span<Vector2> bVertices = stackalloc Vector2[4];
        aTransform.GetTransformedPoints(ref aVertices);
        bTransform.GetTransformedPoints(ref bVertices);

        float minDistance = Single.PositiveInfinity;
        Vector2 bestNormal = Vector2.Zero;
        EdgeConstraint? hitEdge = null;

        for (int i = 0; i < aCollider.edges.Length + bCollider.edges.Length; i++)
        {
            EdgeConstraint edge;

            if (i < aCollider.edges.Length) edge = aCollider.edges[i];
            else edge = bCollider.edges[i - aCollider.edges.Length];
            
            Vector2 axis = new(edge.start.Value.Y - edge.end.Value.Y, edge.end.Value.X - edge.start.Value.X);
            axis.Normalize();

            float distance = SAT.GetOverlapOnAxis(aTransform, bTransform, axis);
            if (distance > 0)
            {
                data = null;
                return false;
            }
            else if (MathF.Abs(distance) < minDistance)
            {
                minDistance = MathF.Abs(distance);
                hitEdge = edge;
                bestNormal = axis;
            }
        }

        if (hitEdge?.parent != bCollider)
        {
            (bCollider, aCollider) = (aCollider, bCollider);
            bestNormal = -bestNormal; // Invert the normal direction
        }

        VerletPoint closestVertex = aCollider.points[0];
        
        
        float smallestD = Single.PositiveInfinity;

        for (int i = 0; i < 4; i++)
        {
            float distance = Vector2.Dot(bestNormal, aCollider.points[i].position.Value - bCollider.Center);
            //float distance = (bestNormal * (aCollider.points[i].position.Value - bCollider.Center)).Length;

            if (distance < smallestD)
            {
                smallestD = distance;

                closestVertex = aCollider.points[i];
            }
        }
        
        data = new CollisionData(minDistance, bestNormal, hitEdge!, closestVertex);

        return true;
    }

    private static float GetOverlapOnAxis(Transform a, Transform b, Vector2 normalizedAxis)
    {
        (float aMin, float aMax) = SAT.GetInterval(a, normalizedAxis);
        (float bMin, float bMax) = SAT.GetInterval(b, normalizedAxis);

        if (aMin < bMin)
        {
            return bMin - aMax;
        }
        else
        {
            return aMin - bMax;
        }
    }

    private static (float, float) GetInterval(Transform box, Vector2 normalizedAxis)
    {
        float min = Single.PositiveInfinity;
        float max = Single.NegativeInfinity;

        Span<Vector2> vertices = stackalloc Vector2[4];
        box.GetTransformedPoints(ref vertices);

        for (int i = 0; i < 4; i++)
        {
            float projection = Vector2.Dot(normalizedAxis, vertices[i]);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }

        return (min, max);
    }
}