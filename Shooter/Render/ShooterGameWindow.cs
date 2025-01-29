using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Shooter.Entities;
using Shooter.Structures;
using Shooter.Render.Shaders;

namespace Shooter.Render;

public class ShooterGameWindow() : GameWindow(
    GameWindowSettings.Default,
    new() { ClientSize = (ShooterGameWindow.PIXELS_X, ShooterGameWindow.PIXELS_Y), Title = "Shooter" }
)
{
    public const int PIXELS_X = 320;
    public const int PIXELS_Y = 180;
    private const float ASPECT_RATIO = 16f / 9f;

    private readonly float[] _fullscreenQuadVerts =
    [ //  Pos      UV
        -1, -1,    0, 0,
         1, -1,    1, 0,
        -1,  1,    0, 1,
         1,  1,    1, 1,
    ];

    private Box _viewport;

    private int _fullRectVbo;
    private int _fullRectVao;

    private readonly Camera _cam = new();

    // No code in this class should run before `OnLoad()`, so it's safe to ignore nullability.
    private Shader _fboDrawer = null!;
    private Framebuffer _fbo = null!;
    private EntityRenderer _entityRenderer = null!;

    protected override void OnLoad()
    {
        base.OnLoad();
        this.WindowState = WindowState.Maximized;
        this._fbo = new();
        
        this._fullRectVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, this._fullRectVbo);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            this._fullscreenQuadVerts.Length * sizeof(float),
            this._fullscreenQuadVerts,
            BufferUsage.StaticDraw
        );
        this._fullRectVao = GL.GenVertexArray();
        GL.BindVertexArray(this._fullRectVao);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

        this._fboDrawer = new("fbo_drawer");
        this._entityRenderer = new();
        Entity test = new Entity()
        {
            Scale = (50,50),
            Pos = (ShooterGameWindow.PIXELS_X / 2f, ShooterGameWindow.PIXELS_Y / 2f)
        };
        this._entityRenderer.AddEntity(test);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        this._fbo.BindFramebuffer();
        GL.Viewport(0, 0, ShooterGameWindow.PIXELS_X, ShooterGameWindow.PIXELS_Y);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        this._entityRenderer.Render(this._cam);

        this._fbo.UnbindFramebuffer();

        GL.Viewport(this._viewport.X, this._viewport.Y, this._viewport.W, this._viewport.H);
        GL.ClearColor(0,0,0,1);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        this._fboDrawer.Use();
        this._fbo.BindTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        this._fbo.BindTexture();

        GL.BindVertexArray(this._fullRectVao);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

        this.SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        float windowAspectRatio = (float)e.Width / e.Height;

        int newWidth, newHeight;

        if (windowAspectRatio > ShooterGameWindow.ASPECT_RATIO)
        {
            newWidth = (int)(e.Height * ShooterGameWindow.ASPECT_RATIO);
            newHeight = e.Height;
        }
        else
        {
            newWidth = e.Width;
            newHeight = (int)(e.Width / ShooterGameWindow.ASPECT_RATIO);
        }

        this._viewport = ((e.Width - newWidth) / 2, (e.Height - newHeight) / 2, newWidth, newHeight);
    }
}