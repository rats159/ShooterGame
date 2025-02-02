using System.Threading.Tasks.Dataflow;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shooter.ECS.Components;
using Shooter.Input;
using Shooter.Render;

namespace Shooter.ECS.Systems;

public class InputTestSystem : ISystem
{
    public void Update(TimeSpan delta)
    {
        List<Entity> entities = 
            ComponentQuery
                .Of<InputControlled>()
                .And<EntityRenderable>()
                .And<Transform>()
                .GetEntities();

        
        foreach (Entity ent in entities)
        {
            if (Mouse.IsButtonDown(MouseButton.Button1))
            {
                InputTestSystem.FollowMouse(ent);
            }
            else
            {
                InputTestSystem.FollowKeyboard(ent);
            }
        }
    }

    private static void FollowMouse(Entity ent)
    {
        Transform transform = ent.Get<Transform>();
        transform.X = (int)(Mouse.X * ShooterGameWindow.SCALE_FACTOR_X - transform.Width / 2f);
        transform.Y = (int)(Mouse.Y * ShooterGameWindow.SCALE_FACTOR_Y - transform.Height / 2f);
    }

    private static void FollowKeyboard(Entity ent)
    {
        Transform transform = ent.Get<Transform>();
        if (Keyboard.IsKeyDown(Keys.W)) transform.Y++;
        if (Keyboard.IsKeyDown(Keys.A)) transform.X--;
        if (Keyboard.IsKeyDown(Keys.S)) transform.Y--;
        if (Keyboard.IsKeyDown(Keys.D)) transform.X++;
    }
}