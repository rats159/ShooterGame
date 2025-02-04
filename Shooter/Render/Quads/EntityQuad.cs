using OpenTK.Graphics.OpenGL;
using Shooter.Assets;

namespace Shooter.Render.Quads;

public class EntityQuad : Quad
{
    private static EntityQuad? _common;
    
    // Most entities can share a quad
    public static EntityQuad Common
    {
        get
        {
            if (EntityQuad._common != null) return EntityQuad._common;
            
            if (!AssetManager.Loaded)
            {
                throw new("EntityQuad.Common attempted to be read before window loaded.");
            }

            EntityQuad._common = new();

            return EntityQuad._common;
        }
    }

    protected override float[] GetVertices()
    {
        return
        [
            -0.5f, -0.5f, 0, 0,
            0.5f, -0.5f, 0, 1,
            -0.5f, 0.5f, 1, 0,
            0.5f, 0.5f, 1, 1
        ];
    }

    protected override void BindAttribs()
    {
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
    }
}