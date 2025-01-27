using OpenTK.Graphics.OpenGL;
using Shooter.Render.Shaders.Parsing;

namespace Shooter.Render.Shaders;

public class Shader
{
    private bool isDisposed;
    private readonly int _handle;
    private readonly string _name;

    public Shader(string name)
    {
        this._name = name;
        string vertexPath = Path.Join("resources", "shader", name + ".vert");
        string fragmentPath = Path.Join("resources", "shader", name + ".frag");

        string vertexSource = File.ReadAllText(vertexPath);
        string fragmentSource = File.ReadAllText(fragmentPath);

        int vertexId = GL.CreateShader(ShaderType.VertexShader);
        int fragmentId = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vertexId, vertexSource);
        GL.ShaderSource(fragmentId, fragmentSource);

        GL.CompileShader(vertexId);

        int success;

        GL.GetShaderi(vertexId, ShaderParameterName.CompileStatus, out success);
        if (success == 0)
        {
            GL.GetShaderInfoLog(vertexId, out string log);
            Console.WriteLine(log);
        }

        GL.CompileShader(fragmentId);

        GL.GetShaderi(fragmentId, ShaderParameterName.CompileStatus, out success);
        if (success == 0)
        {
            GL.GetShaderInfoLog(fragmentId, out string info);
            Console.WriteLine(info);
        }

        this._handle = GL.CreateProgram();

        GL.AttachShader(this._handle, vertexId);
        GL.AttachShader(this._handle, fragmentId);

        GL.LinkProgram(this._handle);

        GL.GetProgrami(this._handle, ProgramProperty.LinkStatus, out success);
        
        if (success == 0)
        {
            GL.GetProgramInfoLog(this._handle, out string log);
            Console.WriteLine(log);
        }
        
        GL.DetachShader(this._handle, vertexId);
        GL.DetachShader(this._handle, fragmentId);
        GL.DeleteShader(vertexId);
        GL.DeleteShader(fragmentId);

        ShaderParser vertexParser = new(vertexSource);
        List<GlslToken> tokens = vertexParser.Parse();
        Console.WriteLine(tokens.Count);
        foreach (GlslToken token in tokens)
        {
            Console.WriteLine(token);
        }
    }
    
    ~Shader()
    {
        if (!this.isDisposed)
        {
            Console.WriteLine($"Shader {this._name} was not cleaned up!");
        }
    }
    
    public void Use()
    {
        GL.UseProgram(this._handle);
    }

    public void Dispose()
    {
        if (this.isDisposed) return;
        
        GL.DeleteProgram(this._handle);
        this.isDisposed = true;
    }
}