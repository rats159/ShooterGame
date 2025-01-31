using Shooter.Render.Quads;

namespace Shooter.ECS.Components;

public class EntityRenderComponent(EntityQuad quad) : IComponent
{
    public readonly EntityQuad quad = quad;
}