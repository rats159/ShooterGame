using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Shooter.Assets;
using Shooter.ECS.Components;
using Shooter.Physics;
using Shooter.Render;
using Shooter.Render.Debug;
using Shooter.Render.Quads;
using Shooter.Render.Shaders;
using Shooter.Structures;
using Shooter.Utility;

namespace Shooter.ECS.Systems;

public class VerletRendererSystem : ISystem
{
    public void Update(TimeSpan delta)
    {
        List<VerletObject> entities = ComponentQuery.Of<VerletObject>().Get<VerletObject>();
        
        foreach (VerletObject obj in entities)
        {
            foreach (EdgeConstraint edge in obj.edges)
            {
                DebugDrawer.DrawLine(edge.start.Value,edge.end.Value,Vector3.One);
            }
        }
    }
}