using Shooter.Structures;

namespace Shooter.ECS.Components;

public class TransformComponent(Transform transform) : IComponent
{
    public TransformComponent(int x, int y, int w, int h, float degrees) : this(new Transform(x, y, w, h, degrees))
    {
    }

    public TransformComponent(Box box) : this(box.X, box.Y, box.W, box.H, 0)
    {
    }

    public Transform transform = transform;
}