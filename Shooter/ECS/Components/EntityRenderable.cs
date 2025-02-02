using Shooter.Render.Quads;

namespace Shooter.ECS.Components;

public class EntityRenderable(EntityQuad quad) : IComponent
{
    public readonly EntityQuad quad = quad;
}