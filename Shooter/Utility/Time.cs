using Shooter.Render;

namespace Shooter.Utility;

public static class Time
{
    public static TimeSpan Delta => ShooterGameWindow.FrameDelta;
}