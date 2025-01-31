using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Shooter.Assets;
using Shooter.ECS.Components;
using Shooter.Render;
using Shooter.Render.Quads;
using Shooter.Render.Shaders;
using Shooter.Structures;
using Shooter.Utility;

namespace Shooter.ECS.Systems;

public class LevelRendererSystem : ISystem
{
    private readonly Shader _shader = AssetManager.GetShader("level");
    private readonly Dictionary<ushort, Matrix4> _transformations = [];
    private readonly Dictionary<ushort, Transform> _cache = [];
    
    public void Update(TimeSpan delta)
    {
        CameraComponent? camera = EntityManager.GetComponent<CameraComponent>();
        if (camera == null)
        {
            Console.WriteLine("No camera!");
            return;
        }


        this._shader.Use();
        this._shader.Load("u_View", camera.View);
        this._shader.Load("u_Proj", camera.Projection);


        List<ushort> entities = EntityManager.GetWithComponents(
            typeof(LevelRenderComponent),
            typeof(TransformComponent),
            typeof(TextureComponent)
        );

        IEnumerable<IGrouping<Texture, ushort>> byTexture =
            entities.GroupBy(id => EntityManager.GetComponent<TextureComponent>(id)!.texture);

        foreach (IGrouping<Texture, ushort> grouping in byTexture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            this._shader.Load("u_Resolution", new Vector2(grouping.Key.Height));
            grouping.Key.Use();
            foreach (ushort entity in grouping)
            {
                Matrix4 mat = this.GetMatrix(entity);
                LevelQuad model = EntityManager.GetComponent<LevelRenderComponent>(entity)!.quad;


                GL.BindVertexArray(
                    model.VaoId
                );
                this._shader.Enable();
                this._shader.Load("u_Transform", mat);
                GL.DrawElements(PrimitiveType.Triangles, model.IndexCount, DrawElementsType.UnsignedInt, 0);
            }
        }
    }

    private Matrix4 GetMatrix(ushort entity)
    {
        TypeMap<IComponent> components = EntityManager.GetComponentsDict(entity);
        Transform transform = components.Get<TransformComponent>().transform;

        if (this._cache.TryGetValue(entity, out Transform value) && value == transform)
            return this._transformations[entity];

        this._transformations[entity] = ExtraMath.TransformationMatrix(transform);
        this._cache[entity] = transform;


        return this._transformations[entity];
    }
}