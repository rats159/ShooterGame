namespace Shooter.ECS;

public readonly record struct Entity(ushort Id)
{
    public bool Has<T>() where T : IComponent => EntityManager.HasComponent<T>(this);

    public T Get<T>() where T : IComponent => EntityManager.GetComponent<T>(this);

    public void AddComponent(IComponent component) => EntityManager.AddComponent(this, component);
}