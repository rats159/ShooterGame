using Shooter.Render;

namespace Shooter;

internal static class Program
{
    public static void Main()
    {
        using ShooterGameWindow game = new();
        game.Run();
    }
}
