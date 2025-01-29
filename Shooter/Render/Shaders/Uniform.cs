using OpenTK.Graphics.OpenGL;

namespace Shooter.Render.Shaders;

public record Uniform(string Name, int Location, UniformType Type);