using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Shooter.Input;

public static class Keyboard
{
    public static KeyboardState State { get; set; }
    
    public static bool IsKeyDown(Keys button)
    {
        return Keyboard.State.IsKeyDown(button);
    }
}