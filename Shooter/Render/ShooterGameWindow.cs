using System.ComponentModel;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shooter.Assets;
using Shooter.ECS;
using Shooter.ECS.Components;
using Shooter.ECS.Systems;
using Shooter.Input;
using Shooter.Levels;
using Shooter.Physics;
using Shooter.Render.Debug;
using Shooter.Render.Quads;
using Shooter.Structures;
using Shooter.Render.Shaders;
using Shooter.Utility;
using StbImageSharp;
using StbImageWriteSharp;

namespace Shooter.Render;

public class ShooterGameWindow() : GameWindow(
    GameWindowSettings.Default,
    new() { Title = "Shooter"}
)
{
    private const bool SAVE_FRAMES_TO_VIDEO = false; 
    private double _startRender;
    
    public static event Action WhenLoaded = () => { };
    public static TimeSpan FrameDelta { get; private set; } = TimeSpan.Zero;

    public const int PIXELS_X = 320;
    public const int PIXELS_Y = 180;
    public static float SCALE_FACTOR_X { get; private set; }
    public static float SCALE_FACTOR_Y { get; private set; }

    public static int WIDTH;
    public static int HEIGHT;

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

    private Pool<byte[]> _pixelPool;

    public static int FrameCount { get; private set; } = 0;



    // No code in this class should run before `OnLoad()`, so it's safe to ignore nullability.
    private Shader _fboDrawer = null!;
    private Framebuffer _fbo = null!;

    protected override void OnLoad()
    {
        base.OnLoad();

        Directory.CreateDirectory("frames");
        Directory.CreateDirectory("video");
        foreach (string file in Directory.GetFiles("frames"))
        {
            File.Delete(file);
        }
        
        AssetManager.Load();
        this.VSync = VSyncMode.On;
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

        if(SAVE_FRAMES_TO_VIDEO)
            this._pixelPool = new(()=>new byte[this.ClientSize.X * this.ClientSize.Y * 4],10);
        
        this._fboDrawer = AssetManager.GetShader("fbo_drawer");
        
        this._systems.Add(new LevelRendererSystem());
        this._systems.Add(new EntityRendererSystem());
        this._systems.Add(new InputTestSystem());
        this._systems.Add(new VerletSystem());
        this._systems.Add(new VerletRendererSystem());
        
        ShooterGameWindow.WhenLoaded.Invoke();
        
        Entity camera = EntityManager.New();
        camera.AddComponent(new Camera());

        for (int i = 0; i < 10; i++)
        {
            Entity e = EntityManager.New();
            e.AddComponent(new VerletObject(i*16+80,120,8,8));
            e.AddComponent(new Transform(i*16+80,120,8,8,0));
        }

        Entity b = EntityManager.New();
        // collider.AddComponent(new InputControlled());
        b.AddComponent(new VerletObject(256,120,8,8));
        b.AddComponent(new Transform(256,120,8,8,0));
        b.AddComponent(new EntityRenderable(EntityQuad.Common));
        // b.AddComponent(new TextureComponent(AssetManager.GetTexture("debug")));
        
        foreach(VerletPoint point in b.Get<VerletObject>().points)
            point.prevPosition.Value.X += 0.5f;
        
        // a.AddComponent(new TextureComponent(AssetManager.GetTexture("debug")));
        
        // c.AddComponent(new TextureComponent(AssetManager.GetTexture("debug")));
        
        
        List<Box> testLevelData =
        [
            (0, 0, ShooterGameWindow.PIXELS_X, 8),
            (0, ShooterGameWindow.PIXELS_Y - 8, ShooterGameWindow.PIXELS_X, 8),
            (ShooterGameWindow.PIXELS_X/2 - 8/2, ShooterGameWindow.PIXELS_Y/2 - 8/2 - 8, 8, 8)
        ];

        Level testLevel = Level.Save(testLevelData);
        Level.Load(testLevel);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if(SAVE_FRAMES_TO_VIDEO)
            this.SaveVideo();
        base.OnClosing(e);
    }

    private void SaveVideo()
    {
        File.Delete("video/out.mp4");
        const string ffmpegPath = "ffmpeg";
        const string arguments = "-framerate 60 -i frames/frame-%05d.png video/out.mp4";

        ProcessStartInfo startInfo = new()
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            UseShellExecute = true
        };
        
        using Process? process = Process.Start(startInfo); 
        process?.WaitForExit();
    }
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        ShooterGameWindow.FrameDelta = TimeSpan.FromSeconds(GLFW.GetTime() - this._startRender);
        this._startRender = GLFW.GetTime();
        base.OnRenderFrame(e);

        Mouse.State = this.MouseState;
        Keyboard.State = this.KeyboardState;
        
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
        
        List<Camera> cameras = ComponentQuery.Of<Camera>().Get<Camera>();
        if (cameras.Count != 0)
        {
            Camera camera = cameras.First();

            DebugDrawer.Render(camera);
        }
        
        if(SAVE_FRAMES_TO_VIDEO)
            this.SaveFrame();
        this.SwapBuffers();
        FrameCount++;
    }

    private void SaveFrame()
    {
        byte[] pixels = this._pixelPool.GetAndBusy();
        GL.ReadPixels(0,0,this.ClientSize.X,this.ClientSize.Y,PixelFormat.Rgba,PixelType.UnsignedByte, pixels);
        Task.Run(() =>
        {
            FileStream? stream = null;
            try
            {
                stream = new($"frames/frame-{ShooterGameWindow.FrameCount.ToString().PadLeft(5, '0')}.png",
                    FileMode.OpenOrCreate);
            
                new ImageWriter().WritePng(pixels,
                    this.ClientSize.X,
                    this.ClientSize.Y,
                    StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha,
                    stream);
            }
            finally
            {
                this._pixelPool.Unbusy(pixels);
                stream?.Close();
            }

            
        });
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        this._viewport = (0, 0, e.Width, e.Height);
        ShooterGameWindow.SCALE_FACTOR_X = ShooterGameWindow.PIXELS_X / (float)e.Width;
        ShooterGameWindow.SCALE_FACTOR_Y = ShooterGameWindow.PIXELS_Y / (float)e.Height;
        ShooterGameWindow.WIDTH = e.Width;
        ShooterGameWindow.HEIGHT = e.Height;
    }

    protected override void OnMouseMove(OpenTK.Windowing.Common.MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        Mouse.OnMouseMove(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        Mouse.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        Mouse.OnMouseUp(e);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        Mouse.OnMouseWheel(e);
    }
}