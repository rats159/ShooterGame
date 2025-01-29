using OpenTK.Graphics.OpenGL;

namespace Shooter.Render.Quads;

public abstract class Quad
{
    // Is this necessary? 
    private readonly uint[] _indices =
    [
        2, 1, 0,
        2, 3, 1
    ];

    public int VaoId { get; }
    public int IndexCount => this._indices.Length;

    protected abstract float[] GetVertices();
    protected abstract void BindAttribs();
    
    protected Quad()
    {
        this.VaoId = GL.GenVertexArray();
        GL.BindVertexArray(this.VaoId);

        int vboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
        float[] vertices = this.GetVertices();
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertices.Length * sizeof(float),
            vertices,
            BufferUsage.StaticDraw
        );

        this.BindAttribs();
        
        int eboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboId);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            this._indices.Length * sizeof(uint),
            this._indices,
            BufferUsage.StaticDraw
        );
    }
    
    
}