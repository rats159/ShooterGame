using Shooter.ECS.Components;
using Shooter.Render;
using Shooter.Render.Quads;

namespace Shooter.ECS.Prefabs;

public static class BasicRenderableEntity
{
    public static Entity Create(Texture texture,int x = 0, int y = 0, int sx = 1, int sy = 1, float degrees = 0)
    {
        Entity entity = EntityManager.New();
        
        entity.AddComponent(new Transform(x,y,sx,sy,degrees));
        entity.AddComponent(new EntityRenderable(EntityQuad.Common));
        entity.AddComponent(new TextureComponent(texture));

        return entity;
    }
}