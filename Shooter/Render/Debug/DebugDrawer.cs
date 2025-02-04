using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Shooter.Assets;
using Shooter.ECS.Components;
using Shooter.Render.Shaders;

namespace Shooter.Render.Debug;

public static class DebugDrawer
{
    private static readonly int Vao;
    private static readonly int Vbo;
    private static readonly List<Vector2> LinePoints = [];
    private static readonly List<Vector3> LineColors = [];

    private static readonly Shader Shader;

    static DebugDrawer()
    {
        DebugDrawer.Vao = GL.GenVertexArray();
        DebugDrawer.Vbo = GL.GenBuffer();
        GL.BindVertexArray(DebugDrawer.Vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, DebugDrawer.Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsage.DynamicDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 2);
        GL.EnableVertexAttribArray(1);
        DebugDrawer.Shader = AssetManager.GetShader("debug");
    }

    public static void DrawLine(Vector2 start, Vector2 end, Vector3 color)
    {
        DebugDrawer.LinePoints.Add(start);
        DebugDrawer.LineColors.Add(color);
        DebugDrawer.LinePoints.Add(end);
        DebugDrawer.LineColors.Add(color);
    }

    public static void DrawLine(Vector2 start, Vector2 end, Vector3 startColor, Vector3 stopColor)
    {
        DebugDrawer.LinePoints.Add(start);
        DebugDrawer.LineColors.Add(startColor);
        DebugDrawer.LinePoints.Add(end);
        DebugDrawer.LineColors.Add(stopColor);
    }
    
    public static void Render(Camera cam)
    {
        DebugDrawer.Shader.Use();
        DebugDrawer.Shader.Load("u_View", cam.View);
        DebugDrawer.Shader.Load("u_Proj", cam.Projection);

        GL.BindVertexArray(DebugDrawer.Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, DebugDrawer.Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            DebugDrawer.LinePoints.Count * sizeof(float) * 2 + DebugDrawer.LineColors.Count * sizeof(float) * 3,
            DebugDrawer.FlattenData(),
            BufferUsage.DynamicDraw);
        
        // GL.BufferSubData(BufferTarget.ArrayBuffer,
        //     0,
        //     DebugDrawer.LinePoints.Count * sizeof(float) * 2,
        //     DebugDrawer.FlattenPoints());
        //
        // GL.BufferSubData(BufferTarget.ArrayBuffer,
        //     DebugDrawer.LinePoints.Count * sizeof(float) * 2,
        //     DebugDrawer.LineColors.Count * sizeof(float) * 3,
        //     DebugDrawer.FlattenColors());

        GL.DrawArrays(PrimitiveType.Lines, 0, DebugDrawer.LinePoints.Count);

        DebugDrawer.LinePoints.Clear();
        DebugDrawer.LineColors.Clear();
    }

    private static float[] FlattenData()
    {
        float[] floats = new float[DebugDrawer.LineColors.Count * 3 + DebugDrawer.LinePoints.Count * 2];
        
        for (int i = 0; i < DebugDrawer.LinePoints.Count; i++)
        {
            floats[i * 5] = DebugDrawer.LinePoints[i].X;
            floats[i * 5 + 1] = DebugDrawer.LinePoints[i].Y;
            floats[i * 5 + 2] = DebugDrawer.LineColors[i].X;
            floats[i * 5 + 3] = DebugDrawer.LineColors[i].Y;
            floats[i * 5 + 4] = DebugDrawer.LineColors[i].Z;
        }

        return floats;
    }
    
}