using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Shooter.Render.Shaders;

public class Shader
{
    private bool isDisposed;
    private readonly int _handle;
    private readonly string _name;
    
    public int NumVertexAttribs { get; }
    private readonly Dictionary<string, Uniform> _uniformLocations = [];

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

        this.NumVertexAttribs = GL.GetProgrami(this._handle, ProgramProperty.ActiveAttributes);
        int maxsize = GL.GetProgrami(this._handle, ProgramProperty.ActiveUniformMaxLength);
        int uniformCount = GL.GetProgrami(this._handle, ProgramProperty.ActiveUniforms);
        
        for (uint i = 0; i < uniformCount; i++)
        {
            GL.GetActiveUniform(this._handle, i, maxsize, out _, out _, out UniformType type,
                out string uniformName);
            int loc = GL.GetUniformLocation(this._handle, uniformName);
            this._uniformLocations[uniformName] = new(uniformName,loc,type);
        }
    }
    
    ~Shader()
    {
        if (!this.isDisposed)
        {
            Console.WriteLine($"Shader {this._name} was not cleaned up!");
        }
    }

    public void Enable()
    {
        for (uint i = 0; i < this.NumVertexAttribs; i++)
        {
            GL.EnableVertexAttribArray(i);
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

    public void LoadMat4(string location, Matrix4 mat)
    {
        if (!this._uniformLocations.TryGetValue(location, out Uniform? value))
        {
            throw new ArgumentException($"Shader {this._name} has no uniform named {location}");
        }

        if (value.Type != UniformType.FloatMat4)
        {
            throw new ArgumentException($"Shader {this._name}'s uniform {value.Name} requires type {value.Type}, not Mat4");
        }
        
        GL.UniformMatrix4f(value.Location,1,false,in mat);
    }
}