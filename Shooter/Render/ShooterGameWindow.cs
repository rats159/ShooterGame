using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shooter.ECS;
using Shooter.ECS.Components;
using Shooter.ECS.Prefabs;
using Shooter.ECS.Systems;
using Shooter.Render.Quads;
using Shooter.Structures;
using Shooter.Render.Shaders;
using Shooter.Utility;

namespace Shooter.Render;

public class ShooterGameWindow() : GameWindow(
    GameWindowSettings.Default,
    new() { ClientSize = (ShooterGameWindow.PIXELS_X, ShooterGameWindow.PIXELS_Y), Title = "Shooter" }
)
{
    private double _startRender;
    
    public static event Action WhenLoaded = () => { };
    public static TimeSpan FrameDelta { get; private set; } = TimeSpan.Zero;

    public const int PIXELS_X = 320;
    public const int PIXELS_Y = 180;
    private const float ASPECT_RATIO = 16f / 9f;

    private readonly List<ISystem> _systems = [];

    private readonly float[] _fullscreenQuadVerts =
    [ //  Pos      UV
        -1, -1, 0, 0,
        1, -1, 1, 0,
        -1, 1, 0, 1,
        1, 1, 1, 1,
    ];

    private Box _viewport;

    private int _fullRectVbo;
    private int _fullRectVao;


    // No code in this class should run before `OnLoad()`, so it's safe to ignore nullability.
    private Shader _fboDrawer = null!;
    private Framebuffer _fbo = null!;

    protected override void OnLoad()
    {
        base.OnLoad();
        this.WindowState = WindowState.Maximized;
        this.VSync = VSyncMode.On;
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
        this._systems.Add(new RendererSystem());
        this._systems.Add(new SpinnyTestSystem());

        _ = EntityQuad.Common;

        ShooterGameWindow.WhenLoaded.Invoke();

        Texture tex = new("face");
        
        for(int j = -3; j < 4; j++)
        {
            for (int i = -5; i < 6; i++)
            {
                BasicRenderableEntity.Create(
                    tex,
                    ShooterGameWindow.PIXELS_X / 2f + i * 32,
                    ShooterGameWindow.PIXELS_Y / 2f + j * 32,
                    16,
                    16
                );
            }
        }
        ushort camera = EntityManager.New();
        EntityManager.AddComponent(camera, new CameraComponent());
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        ShooterGameWindow.FrameDelta = TimeSpan.FromSeconds(GLFW.GetTime() - this._startRender);
        Console.WriteLine(FrameDelta.TotalMilliseconds);
        this._startRender = GLFW.GetTime();
        base.OnRenderFrame(e);

        this._fbo.BindFramebuffer();
        GL.Viewport(0, 0, ShooterGameWindow.PIXELS_X, ShooterGameWindow.PIXELS_Y);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        foreach (ISystem system in this._systems)
        {
            system.Update(Time.Delta);
        }

        this._fbo.UnbindFramebuffer();

        GL.Viewport(this._viewport.X, this._viewport.Y, this._viewport.W, this._viewport.H);
        GL.ClearColor(0, 0, 0, 1);
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