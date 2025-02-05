using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Shooter.Assets;
using Shooter.ECS.Components;
using Shooter.Render.Shaders;

namespace Shooter.Render.Debug;

public static class DebugDrawer
{
    private static readonly int LineVao;
    private static readonly int LineVbo;
    private static readonly List<Vector2> LinePoints = [];
    private static readonly List<Vector3> LineColors = [];

    
    private static readonly int PointVao;
    private static readonly int PointVbo;
    private static readonly List<Vector2> PointPoints = [];
    private static readonly List<Vector3> PointColors = [];

    private static readonly Shader Shader;

    static DebugDrawer()
    {
        DebugDrawer.LineVao = GL.GenVertexArray();
        DebugDrawer.LineVbo = GL.GenBuffer();
        GL.BindVertexArray(DebugDrawer.LineVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, DebugDrawer.LineVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsage.DynamicDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 2);
        GL.EnableVertexAttribArray(1);
        
        
        DebugDrawer.PointVao = GL.GenVertexArray();
        DebugDrawer.PointVbo = GL.GenBuffer();
        GL.BindVertexArray(DebugDrawer.PointVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, DebugDrawer.PointVbo);
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
    
    public static void DrawPoint(Vector2 pos, Vector3 color)
    {
        DebugDrawer.PointPoints.Add(pos);
        DebugDrawer.PointColors.Add(color);
    }

    public static void DrawLine(Vector2 start, Vector2 end, Vector3 startColor, Vector3 stopColor)
    {
        DebugDrawer.LinePoints.Add(start);
        DebugDrawer.LineColors.Add(startColor);
        DebugDrawer.LinePoints.Add(end);
        DebugDrawer.LineColors.Add(stopColor);
    }

    public static void Erase()
    {
        DebugDrawer.LinePoints.Clear();
        DebugDrawer.LineColors.Clear();
        DebugDrawer.PointPoints.Clear();
        DebugDrawer.PointColors.Clear();
    }
    
    public static void Render(Camera cam)
    {
        DebugDrawer.Shader.Use();
        DebugDrawer.Shader.Load("u_View", cam.View);
        DebugDrawer.Shader.Load("u_Proj", cam.Projection);

        GL.BindVertexArray(DebugDrawer.LineVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, DebugDrawer.LineVbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            DebugDrawer.LinePoints.Count * sizeof(float) * 2 + DebugDrawer.LineColors.Count * sizeof(float) * 3,
            DebugDrawer.FlattenLineData(),
            BufferUsage.DynamicDraw);
        GL.PointSize(1);
        GL.DrawArrays(PrimitiveType.Lines, 0, DebugDrawer.LinePoints.Count);

        GL.BindVertexArray(DebugDrawer.PointVao);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, DebugDrawer.PointVbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            DebugDrawer.PointPoints.Count * sizeof(float) * 2 + DebugDrawer.PointColors.Count * sizeof(float) * 3,
            DebugDrawer.FlattenPointData(),
            BufferUsage.DynamicDraw);
        
        GL.PointSize(5);
        GL.DrawArrays(PrimitiveType.Points, 0, DebugDrawer.PointPoints.Count);

        
        DebugDrawer.Erase();
    }

    private static float[] FlattenPointData()
    {
        float[] floats = new float[DebugDrawer.PointColors.Count * 3 + DebugDrawer.PointPoints.Count * 2];
        
        for (int i = 0; i < DebugDrawer.PointPoints.Count; i++)
        {
            floats[i * 5] = DebugDrawer.PointPoints[i].X;
            floats[i * 5 + 1] = DebugDrawer.PointPoints[i].Y;
            floats[i * 5 + 2] = DebugDrawer.PointColors[i].X;
            floats[i * 5 + 3] = DebugDrawer.PointColors[i].Y;
            floats[i * 5 + 4] = DebugDrawer.PointColors[i].Z;
        }

        return floats;
    }
    
    private static float[] FlattenLineData()
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