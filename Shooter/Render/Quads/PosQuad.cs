using OpenTK.Graphics.OpenGL;

namespace Shooter.Render.Quads;

public class PosQuad : Quad
{
    protected override float[] GetVertices()
    {
        return [
            0,0,
            1,0,
            0,1,
            1,1
        ];
    }

    protected override void BindAttribs()
    {
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

    }
}