using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using Shooter.Render;

namespace Shooter.Input;

public static class Mouse
{
    public static event Action<MouseButtonEventArgs>? MouseDown;
    public static event Action<MouseButtonEventArgs>? MouseUp;
    public static event Action<MouseWheelEventArgs>? MouseWheel;
    public static event Action<MouseMoveEventArgs>? MouseMove;

    public static MouseState State { get; set; }

    public static float X => Mouse.State.Position.X;
    public static float Y => ShooterGameWindow.HEIGHT - Mouse.State.Position.Y;

    public static Vector2 Pos => Mouse.State.Position;

    public static bool IsButtonDown(MouseButton button)
    {
        return Mouse.State.IsButtonDown(button);
    }
    
    public static void OnMouseDown(MouseButtonEventArgs e)
    {
        Mouse.MouseDown?.Invoke(e);
    }

    public static void OnMouseUp(MouseButtonEventArgs e)
    {
        Mouse.MouseUp?.Invoke(e);
    }

    public static void OnMouseWheel(MouseWheelEventArgs e)
    {
        Mouse.MouseWheel?.Invoke(e);
    }

    public static void OnMouseMove(OpenTK.Windowing.Common.MouseMoveEventArgs e)
    {
        Mouse.MouseMove?.Invoke(e);
    }
}