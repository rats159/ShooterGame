using Shooter.ECS.Components;
using Shooter.Render;
using Shooter.Render.Quads;

namespace Shooter.ECS.Prefabs;

public static class BasicRenderableEntity
{
    public static ushort Create(Texture texture,int x = 0, int y = 0, int sx = 1, int sy = 1, float degrees = 0)
    {
        ushort id = EntityManager.New();
        
        EntityManager.AddComponent(id,new TransformComponent(x,y,sx,sy,degrees));
        EntityManager.AddComponent(id,new EntityRenderComponent(EntityQuad.Common));
        EntityManager.AddComponent(id,new TextureComponent(texture));

        return id;
    }
}