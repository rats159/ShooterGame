using OpenTK.Graphics.OpenGL;

namespace Shooter.Render.Quads;

public class UvQuad : Quad
{
    protected override float[] GetVertices()
    {
        return [
            0,0,  0,0,
            1,0,  0,1,
            0,1,  1,0,
            1,1,  1,1
        ];
    }

    protected override void BindAttribs()
    {
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2* sizeof(float));
    }
}