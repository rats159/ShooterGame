using OpenTK.Graphics.OpenGL;

namespace Shooter.Render;

public class Quad
{
    private readonly float[] _vertices =
    [
        0f, 0f,
        0f, 1f,
        1f, 0f,
        1f, 1f,
    ];

    // Is this necessary? 
    private readonly uint[] _indices =
    [
        2, 1, 0,
        2, 3, 1
    ];

    public int VaoId { get; }
    public int IndexCount => this._indices.Length;

    public Quad()
    {
        this.VaoId = GL.GenVertexArray();
        GL.BindVertexArray(this.VaoId);

        int vboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            this._vertices.Length * sizeof(float),
            this._vertices,
            BufferUsage.StaticDraw
        );

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

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