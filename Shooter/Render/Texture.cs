using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Shooter.Render;

public class Texture
{
    public readonly int handle;
    public Texture(string name)
    {
        this.handle = GL.GenTexture();
        this.Use();
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult image = ImageResult.FromStream(File.OpenRead(Path.Join("resources","texture",$"{name}.png")), ColorComponents.RedGreenBlueAlpha);
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    public void Use()
    {
        GL.BindTexture(TextureTarget.Texture2d,this.handle);
    }
}