using Shooter.ECS.Components;

namespace Shooter.ECS.Systems;

public class SpinnyTestSystem : ISystem
{
    public void Update(TimeSpan delta)
    {
        foreach (RotationComponent rot in EntityManager.GetComponents<RotationComponent>())
        {
            rot.Turn((float)delta.TotalMilliseconds / 5f);
        }
    }
}