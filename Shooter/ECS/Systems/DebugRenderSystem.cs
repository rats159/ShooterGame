using Shooter.ECS.Components;
using Shooter.Render.Debug;

namespace Shooter.ECS.Systems;

public class DebugRenderSystem : ISystem
{
    public void Update(TimeSpan delta)
    {
        List<Camera> cameras = ComponentQuery.Of<Camera>().Get<Camera>();
        if (cameras.Count == 0) return;
        Camera camera = cameras.First();
        
        DebugDrawer.Render(camera);
    }
}