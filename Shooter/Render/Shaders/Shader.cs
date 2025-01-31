using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;
using Shooter.Assets;

namespace Shooter.Render.Shaders;

public class Shader
{
    private bool _isDisposed;
    private readonly int _handle;
    private readonly string _name;

    public int NumVertexAttribs { get; }
    private readonly Dictionary<string, Uniform> _uniformLocations = [];

    public Shader(string vertexPath, string fragmentPath)
    {
        if (AssetManager.Loaded)
        {
            Console.WriteLine("[WARN] Shader created while assets already loaded. Perhaps use `AssetManager.GetShader(name)` instead?");
        }
        
        this._name = Path.GetFileNameWithoutExtension(vertexPath);

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
            Environment.Exit(1);
        }

        GL.CompileShader(fragmentId);

        GL.GetShaderi(fragmentId, ShaderParameterName.CompileStatus, out success);
        if (success == 0)
        {
            GL.GetShaderInfoLog(fragmentId, out string info);
            Console.WriteLine(info);
            Environment.Exit(1);
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
            Environment.Exit(1);
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
            GL.GetActiveUniform(
                this._handle,
                i,
                maxsize,
                out _,
                out _,
                out UniformType type,
                out string uniformName
            );
            int loc = GL.GetUniformLocation(this._handle, uniformName);
            this._uniformLocations[uniformName] = new(uniformName, loc, type);
        }
    }

    ~Shader()
    {
        if (!this._isDisposed)
        {
            Console.WriteLine($"Shader {this._name}({this._handle}) was not cleaned up!");
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
        if (this._isDisposed) return;

        GL.DeleteProgram(this._handle);
        this._isDisposed = true;
    }

    public void Load<T>(string location, T thing)
    {
        if (!this._uniformLocations.TryGetValue(location, out Uniform? uniform))
        {
            throw new ArgumentException($"Shader `{this._name}` has no uniform named {location}");
        }

        switch (thing)
        {
            case Matrix4 mat: this.LoadMat4(mat, uniform); break;
            case Vector2 vec: this.LoadVec2(vec, uniform); break;
            case float f: this.LoadFloat(f, uniform); break;
            case int i: this.LoadInt(i, uniform); break;
            default: throw new ArgumentException($"{typeof(T)} is not a valid type to be uploaded to shaders.");
        }
    }

    private void LoadFloat(float f, Uniform uniform)
    {
        if (uniform.Type != UniformType.Float)
        {
            throw this.UniformTypeErrorMaker<float>(uniform);
        }

        GL.Uniform1f(uniform.Location, f);
    }
    
    private void LoadInt(int i, Uniform uniform)
    {
        if (uniform.Type != UniformType.Int)
        {
            throw this.UniformTypeErrorMaker<int>(uniform);
        }

        GL.Uniform1i(uniform.Location, i);
    }

    private void LoadMat4(Matrix4 mat, Uniform uniform)
    {
        if (uniform.Type != UniformType.FloatMat4)
        {
            throw this.UniformTypeErrorMaker<Matrix4>(uniform);
        }

        GL.UniformMatrix4f(uniform.Location, 1, false, in mat);
    }

    private void LoadVec2(Vector2 vec, Uniform uniform)
    {
        if (uniform.Type != UniformType.FloatVec2)
        {
            throw this.UniformTypeErrorMaker<Vector2>(uniform);
        }

        GL.Uniform2f(uniform.Location, vec.X, vec.Y);
    }

    private ArgumentException UniformTypeErrorMaker<T>(Uniform uniform)
    {
        return new($"Shader {this._name}'s uniform {uniform.Name} requires type {uniform.Type}, not {typeof(T).Name}");
    }
}