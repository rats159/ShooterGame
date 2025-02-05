using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shooter.ECS.Components;
using Shooter.Input;
using Shooter.Physics;
using Shooter.Render.Debug;
using Shooter.Utility;

namespace Shooter.ECS.Systems;

public class VerletSystem : ISystem
{
    private static readonly Vector2 GRAVITY = (0, -0f);
    private static readonly int SIM_SUBSTEPS = 1;
    private static readonly int COLLISION_SUBSTEPS = 4;
    private static readonly int EDGE_FIXING_SUBSTEPS = 4;


    public void Update(TimeSpan delta)
    {
        List<Entity> entities = ComponentQuery.Of<VerletObject>().And<Transform>().GetEntities();

        foreach (Entity entity in entities)
        {
            VerletObject verletData = entity.Get<VerletObject>();

            if (!verletData.isStatic)
            {
                float trueDelta = (float)delta.TotalSeconds;
                for (int i = 0; i < VerletSystem.SIM_SUBSTEPS; i++)
                {
                    foreach (VerletPoint point in verletData.points)
                    {
                        VerletSystem.ApplyGravity(point);
                        VerletSystem.UpdatePosition(point, trueDelta / VerletSystem.SIM_SUBSTEPS);
                    }
                }
                foreach (VerletPoint p in verletData.points)
                {
                    DebugDrawer.DrawPoint(p.position.Value,(0,0,1));
                }

                for (int i = 0; i < VerletSystem.COLLISION_SUBSTEPS; i++)
                {
                    foreach (Entity other in entities)
                    {
                        if (entity == other) continue;
                        VerletObject otherObject = other.Get<VerletObject>();


                        if (SAT.IsColliding(entity, other, out CollisionData? info))
                        {
                            CollisionData data = info!.Value;
                            VerletSystem.ProcessCollision(verletData, otherObject, ref data);
                        }
                    }
                }
            }

            for (int i = 0; i < EDGE_FIXING_SUBSTEPS; i++)
            {
                VerletSystem.FixEdges(verletData);
            }


            Transform transform = entity.Get<Transform>();
            Vector2 v0Pos = verletData.v0.position.Value;
            Vector2 v1Pos = verletData.v1.position.Value;
            transform.X = (int)v0Pos.X;
            transform.Y = (int)v1Pos.Y;
            transform.Degrees = MathHelper.RadiansToDegrees(MathF.Atan2((v1Pos.Y - v0Pos.Y), (v1Pos.X - v0Pos.X)));
        }
    }

    private static void FixEdges(VerletObject data)
    {
        foreach (EdgeConstraint edge in data.edges)
        {
            Vector2 V1V2 = edge.end.Value - edge.start.Value;

            float V1V2Length = V1V2.Length;
            float Diff = V1V2Length - edge.desiredLength;


            V1V2.Normalize();

            edge.start.Value += V1V2 * Diff * 0.5f;
            edge.end.Value -= V1V2 * Diff * 0.5f;
        }
    }

    private static void ApplyGravity(VerletPoint verlet)
    {
        verlet.acceleration.Value += VerletSystem.GRAVITY;
    }

    private static void UpdatePosition(VerletPoint verlet, float dt)
    {
        Vector2 vel = verlet.position.Value - verlet.prevPosition.Value;
        verlet.prevPosition.Value = verlet.position.Value;

        verlet.position.Value = verlet.position.Value + vel + verlet.acceleration.Value * dt * dt;

        verlet.acceleration.Value = Vector2.Zero;
    }
    
    private static void ProcessCollision(VerletObject self, VerletObject hitObject, ref CollisionData info)
    {
        if (self.isStatic)
        {
            return;
        }
        
        DebugDrawer.Erase();
        
        DebugDrawer.DrawLine(info.HitEdge.start.Value,info.HitEdge.end.Value,(1,0,0));

        Vector2 edgeMidpoint = (info.HitEdge.start.Value + info.HitEdge.end.Value) / 2;
        DebugDrawer.DrawLine(edgeMidpoint,edgeMidpoint + info.Normal * 4,(0,1,0));
        

        Vector2 collisionVector = info.Normal * info.Depth;

        StructRef<Vector2> e1 = info.HitEdge.start;
        StructRef<Vector2> e2 = info.HitEdge.end;

        ref Vector2 hitEdgeStart = ref e1.Value;
        ref Vector2 hitEdgeEnd = ref e2.Value;
        
        float t;

        if (MathF.Abs(hitEdgeStart.X - hitEdgeEnd.X) > MathF.Abs(hitEdgeStart.Y - hitEdgeEnd.Y))
            t = (info.HitVert.position.Value.X - collisionVector.X - hitEdgeStart.X) / (hitEdgeEnd.X - hitEdgeStart.X);
        else
            t = (info.HitVert.position.Value.Y - collisionVector.Y - hitEdgeStart.Y) / (hitEdgeEnd.Y - hitEdgeStart.Y);
        
        float lambda = 1.0f / (t * t + (1 - t) * (1 - t));
        if (t == 0) lambda = 0;
        
        if(!hitObject.isStatic){
            hitEdgeStart -= collisionVector * (1 - t) * 0.5f * lambda;
            hitEdgeEnd -= collisionVector * t * 0.5f * lambda;
            info.HitVert.position.Value += collisionVector * 0.5f;
        }
        else
        {
            info.HitVert.position.Value -= collisionVector;
        }
    }
}