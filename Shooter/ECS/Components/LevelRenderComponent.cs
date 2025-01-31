using Shooter.Render.Quads;

namespace Shooter.ECS.Components;

public class LevelRenderComponent(LevelQuad quad) : IComponent
{
    public readonly LevelQuad quad = quad;
}