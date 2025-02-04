using Shooter.ECS;
using Shooter.ECS.Components;
using Shooter.Render;
using Shooter.Structures;

namespace Shooter;

internal static class Program
{
    public static void Main()
    {
        using ShooterGameWindow game = new();
        game.Run();
    }
}