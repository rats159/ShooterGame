using Shooter.ECS.Components;
using Shooter.Render.Quads;

namespace Shooter.ECS.Prefabs;

public static class BasicRenderableEntity
{
    public static ushort Create(float x = 0, float y = 0, float sx = 1, float sy = 1, int degrees = 0)
    {
        ushort id = EntityManager.New();
        
        EntityManager.AddComponent(id,new PositionComponent(x,y));
        EntityManager.AddComponent(id,new QuadComponent(EntityQuad.Common));
        EntityManager.AddComponent(id,new ScaleComponent(sx,sy));
        EntityManager.AddComponent(id,new RotationComponent(degrees));
        EntityManager.AddComponent(id,new RenderComponent());

        return id;
    }
}