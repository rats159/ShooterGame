using OpenTK.Graphics.OpenGL;

namespace Shooter.Render;

public class Framebuffer
{
    private readonly int _fboId;
    private readonly int _textureId;

    private readonly byte[] _textureData = new byte[ShooterGameWindow.PIXELS_X * ShooterGameWindow.PIXELS_Y * 3];
    
    public Framebuffer()
    {
        for (int i = 0; i < this._textureData.Length; i+=3)
        {
            this._textureData[i] = 255;
            this._textureData[i+1] = 0;
            this._textureData[i+2] = 0;
        }

        this._fboId = GL.GenFramebuffer();
        this.BindFramebuffer();
        
        this._textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, this._textureId);
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgb, ShooterGameWindow.PIXELS_X,
            ShooterGameWindow.PIXELS_Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, this._textureData);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2d, this._textureId, 0);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
        {
            throw new("I don't know what this is");
        }
    }

    public void BindFramebuffer()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._fboId);
    }

    public void UnbindFramebuffer()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void BindTexture()
    {
        GL.BindTexture(TextureTarget.Texture2d,this._textureId);
    }
}