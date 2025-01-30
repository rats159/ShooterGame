using Shooter.Render.Quads;

namespace Shooter.ECS.Components;

public class QuadComponent(Quad quad) : IComponent
{
    public readonly Quad quad = quad;
}