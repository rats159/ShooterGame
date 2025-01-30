using OpenTK.Graphics.OpenGL;

namespace Shooter.Render.Quads;

public class EntityQuad : Quad
{
    static EntityQuad()
    {
        Console.WriteLine("static constructor!");
        ShooterGameWindow.WhenLoaded += EntityQuad.LoadEvent;
    }

    private static void LoadEvent()
    {
        Console.WriteLine("HELLO!");
        EntityQuad.Common = new();
        ShooterGameWindow.WhenLoaded -= EntityQuad.LoadEvent;
    }

    // Most entities can share a quad
    public static EntityQuad Common { get; private set; } = null!;

    protected override float[] GetVertices()
    {
        return
        [
            -.5f, -.5f, 0, 0,
             .5f, -.5f, 0, 1,
            -.5f,  .5f, 1, 0,
             .5f,  .5f, 1, 1
        ];
    }

    protected override void BindAttribs()
    {
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
    }
}