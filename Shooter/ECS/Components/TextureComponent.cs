using Shooter.Render;

namespace Shooter.ECS.Components;

public class TextureComponent(Texture texture) : IComponent
{
    public Texture texture = texture;
}