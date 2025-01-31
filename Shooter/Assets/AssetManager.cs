using System.Text;
using Shooter.Render;
using Shooter.Render.Shaders;
using StbImageSharp;

namespace Shooter.Assets;

public static class AssetManager
{
    public static bool Loaded { get; private set; } = false;
    private static readonly Dictionary<string, Texture> Textures = [];
    
    // I'm not sure if only allowing one instance of a each shader causes any limitations, but i can't think of any 
    private static readonly Dictionary<string, Shader> Shaders = [];

    public static void Load()
    {
        AssetManager.LoadTextures();
        AssetManager.LoadShaders();

        AssetManager.Loaded = true;
    }

    private static void LoadShaders()
    {
        string shaderPath = Path.Join("resources", "shader");
        string[] verts = Directory.GetFiles(shaderPath, "*.vert");
        string[] frags = Directory.GetFiles(shaderPath, "*.frag");
        
        HashSet<string> vertexNames = [..verts.Select(Path.GetFileNameWithoutExtension)!];
        HashSet<string> fragmentNames = [..frags.Select(Path.GetFileNameWithoutExtension)!];

        if (vertexNames.Intersect(fragmentNames).Count() != frags.Length)
        {
            //Bad for performance, but it's fine since we're 99% sure to be crashing

            string[] missingVerts = vertexNames.Where(v => !fragmentNames.Contains(v)).ToArray();
            string[] missingFrags = fragmentNames.Where(f => !vertexNames.Contains(f)).ToArray();

            StringBuilder error = new();

            if (missingFrags.Length != 0 && missingVerts.Length != 0)
            {
                error.AppendLine(
                    $"Shader Mismatch. Found {missingVerts.Length} vertex shader(s) and {missingFrags.Length} fragment shader(1) with no match"
                );
            }
            else if (missingFrags.Length != 0)
            {
                error.AppendLine(
                    $"Shader Mismatch. Found {missingFrags.Length} fragment shaders(s) with no match"
                );
            }
            else
            {
                error.AppendLine(
                    $"Shader Mismatch. Found {missingVerts.Length} shader(s) with no match"
                );
            }

            if (missingFrags.Length != 0)
            {
                error.AppendLine("Fragment shaders with no match:");
                foreach (string name in missingFrags)
                {
                    error.Append('\t').AppendLine(name);
                }
            }
            
            if (missingVerts.Length != 0)
            {
                error.AppendLine("Vertex shaders with no match:");
                foreach (string name in missingVerts)
                {
                    error.Append('\t').AppendLine(name);
                }
            }



            throw new(error.ToString());
        }
        
        foreach (string vert in verts)
        {
            string name = Path.GetFileNameWithoutExtension(vert);
            AssetManager.Shaders[name] = new(Path.Join(shaderPath,$"{name}.vert"),Path.Join(shaderPath,$"{name}.frag"));
        }
    }

    private static void LoadTextures()
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        string[] files = Directory.GetFiles(Path.Join("resources", "texture"), "*.png");

        foreach (string file in files)
        {
            string simpleName = Path.GetFileNameWithoutExtension(file);

            AssetManager.Textures[simpleName] = new(file);
        }
    }


    public static Texture GetTexture(string name)
    {
        if (!AssetManager.Loaded)
        {
            throw new(
                $"Texture {name} attempted to be read before assets were loaded. Assets are loaded as soon as a Window's loading is complete, before any entities are created."
            );
        }

        if (AssetManager.Textures.TryGetValue(name, out Texture? texture))
        {
            return texture;
        }

        throw new KeyNotFoundException($"Texture `{name}` not found.");
    }
    
    public static Shader GetShader(string name)
    {
        if (!AssetManager.Loaded)
        {
            throw new(
                $"Shader {name} attempted to be read before assets were loaded. Assets are loaded as soon as a Window's loading is complete, before any entities are created."
            );
        }

        if (AssetManager.Shaders.TryGetValue(name, out Shader? shader))
        {
            return shader;
        }

        throw new KeyNotFoundException($"Shader `{name}` not found.");
    }
}