using OpenTK.Graphics.OpenGL;
using Shooter.Assets;
using StbImageSharp;

namespace Shooter.Render;

public class Texture
{
    private readonly int _handle;
    public int Width { get; }
    public int Height { get; }
    
    public Texture(string path)
    {
        if (AssetManager.Loaded)
        {
            Console.WriteLine("[WARN] Texture created while assets already loaded. Perhaps use `AssetManager.GetTexture(name)` instead?");
        }
        
        this._handle = GL.GenTexture();
        this.Use();
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        this.Width = image.Width;
        this.Height = image.Height;
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    public void Use()
    {
        GL.BindTexture(TextureTarget.Texture2d,this._handle);
    }
}