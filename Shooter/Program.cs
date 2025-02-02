using Shooter.ECS;
using Shooter.ECS.Components;
using Shooter.Render;
using Shooter.Structures;

namespace Shooter;

internal static class Program
{
    public static void Main()
    {
        IEnumerable<Entity> transforms =
            from entity in EntityManager.Entities
            where entity.Has<Transform>()
            where entity.Has<EntityRenderable>()
            select entity;

        using ShooterGameWindow game = new();
        game.Run();
    }
}