using System.Data.SqlTypes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Shooter.ECS.Components;
using Shooter.Render.Shaders;
using Shooter.Utility;

namespace Shooter.ECS.Systems;

public class RendererSystem : ISystem
{
    private Shader _shader = new("entity");
    private Dictionary<ushort, Matrix4> _transformations = [];
    private Dictionary<ushort, (float, float, float, float, float)> _cache = [];

    public void Update(TimeSpan delta)
    {
        CameraComponent? camera = EntityManager.GetComponent<CameraComponent>();
        if (camera == null)
        {
            Console.WriteLine("No camera!");
            return;
        }


        this._shader.Use();
        this._shader.LoadMat4("u_View", camera.View);
        this._shader.LoadMat4("u_Proj", camera.Projection);


        List<ushort> entities = EntityManager.GetWithComponents(
            typeof(PositionComponent),
            typeof(QuadComponent),
            typeof(RenderComponent),
            typeof(ScaleComponent),
            typeof(RotationComponent)
        );

        Console.WriteLine(entities.Count);
        
        foreach (ushort entity in entities)
        {
            Matrix4 mat = this.GetMatrix(entity);
            QuadComponent model = EntityManager.GetComponent<QuadComponent>(entity)!;
            
            
            GL.BindVertexArray(
                model.
                    quad.
                    VaoId);
            this._shader.Enable();
            this._shader.LoadMat4("u_Transform", mat);
            GL.DrawElements(PrimitiveType.Triangles, model.quad.IndexCount, DrawElementsType.UnsignedInt, 0);
        }
    }

    private Matrix4 GetMatrix(ushort entity)
    {
        TypeMap<IComponent> components = EntityManager.GetComponentsDict(entity);
        (float x, float y) = components.Get<PositionComponent>().XY;
        float degrees = components.Get<RotationComponent>().degrees;
        (float sx, float sy) = components.Get<ScaleComponent>().XY;


        if (this._cache.TryGetValue(entity, out var value) && value.CompareTo((x, y, sx, sy, degrees)) == 0)
            return this._transformations[entity];

        this._transformations[entity] = RendererSystem.MakeMatrix(x, y, sx, sy, degrees);
        this._cache[entity] = (x, y, sx, sy, degrees);


        return this._transformations[entity];
    }

    private static Matrix4 MakeMatrix(float x, float y, float sx, float sy, float degrees)
    {
        Matrix4 mat = Matrix4.CreateScale(sx, sy, 1);
        mat *= Matrix4.CreateRotationZ(degrees);
        mat *= Matrix4.CreateTranslation(x, y, 0);
        return mat;
    }
}