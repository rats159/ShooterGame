using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shooter.Entities;
using Shooter.Render.Shaders;

namespace Shooter.Render;

public class EntityRenderer
{
    private readonly Shader _shader = new("entity");
    private readonly List<Entity> _ents = [];

    public void AddEntity(Entity ent)
    {
        this._ents.Add(ent);
    }
    
    public void Render(Camera camera)
    {
        this._shader.Use();
        this._shader.LoadMat4("u_View",camera.View);
        this._shader.LoadMat4("u_Proj",camera.Projection);
        
        foreach(Entity ent in this._ents)
        {
            GL.BindVertexArray(ent.Model.VaoId);
            this._shader.Enable();
            this._shader.LoadMat4("u_Transform", ent.ModelMatrix);
            GL.DrawElements(PrimitiveType.Triangles, ent.Model.IndexCount,DrawElementsType.UnsignedInt, 0);
        }
        
    }
}