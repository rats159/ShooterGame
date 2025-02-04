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
            InputTestSystem.FollowKeyboard(ent);
        }
    }

    private static void FollowKeyboard(Entity ent)
    {
        Transform transform = ent.Get<Transform>();
        if (Keyboard.IsKeyDown(Keys.W)) transform.Y++;
        if (Keyboard.IsKeyDown(Keys.A)) transform.X--;
        if (Keyboard.IsKeyDown(Keys.S)) transform.Y--;
        if (Keyboard.IsKeyDown(Keys.D)) transform.X++;

        if (Keyboard.IsKeyDown(Keys.Q)) transform.Degrees += 1;
        if (Keyboard.IsKeyDown(Keys.E)) transform.Degrees -= 1;
    }
}