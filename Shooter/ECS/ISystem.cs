namespace Shooter.ECS;

public interface ISystem
{
    public void Update(TimeSpan delta);
}