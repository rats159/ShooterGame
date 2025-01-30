namespace Shooter.ECS.Components;

public class RotationComponent(float degrees = 0) : IComponent
{
    public float degrees = degrees;

    public void Turn(float change)
    {
        this.degrees += change;
    }
}