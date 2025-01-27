using OpenTK.Graphics.OpenGL;

namespace Shooter.Render;

public class Renderer(Shader shader)
{
    private readonly List<Quad> _quads = [];

    public void AddQuad(Quad q)
    {
        this._quads.Add(q);
    }
    
    public void Render()
    {
        shader.Use();
        
        foreach(Quad quad in this._quads)
        {
            GL.BindVertexArray(quad.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.DrawElements(PrimitiveType.Triangles, quad.IndexCount,DrawElementsType.UnsignedInt, 0);
        }
        
    }
}