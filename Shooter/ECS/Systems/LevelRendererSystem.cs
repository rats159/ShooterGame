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
    private readonly Dictionary<Entity, Matrix4> _transformations = [];
    private readonly Dictionary<Entity, Transform> _cache = [];

    public void Update(TimeSpan delta)
    {
        if(!EntityManager.ComponentExists<Camera>())
        {
            Console.WriteLine("No camera!");
            return;
        }

        Camera camera = EntityManager.GetComponent<Camera>();


        this._shader.Use();
        this._shader.Load("u_View", camera.View);
        this._shader.Load("u_Proj", camera.Projection);


        List<Entity> entities = ComponentQuery
            .Of<LevelRenderComponent>()
            .And<Transform>()
            .And<TextureComponent>()
            .GetEntities();

        IEnumerable<IGrouping<Texture, Entity>> byTexture =
            entities.GroupBy(ent => ent.Get<TextureComponent>().texture);

        foreach (IGrouping<Texture, Entity> grouping in byTexture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            this._shader.Load("u_Resolution", new Vector2(grouping.Key.Height));
            grouping.Key.Use();
            foreach (Entity entity in grouping)
            {
                Matrix4 mat = this.GetMatrix(entity);
                LevelQuad model = entity.Get<LevelRenderComponent>().quad;


                GL.BindVertexArray(
                    model.VaoId
                );
                this._shader.Enable();
                this._shader.Load("u_Transform", mat);
                GL.DrawElements(PrimitiveType.Triangles, model.IndexCount, DrawElementsType.UnsignedInt, 0);
            }
        }
    }

    private Matrix4 GetMatrix(Entity entity)
    {
        Transform transform = entity.Get<Transform>();

        if (this._cache.TryGetValue(entity, out Transform? value) && value == transform)
        {
            return this._transformations[entity];
        }
        
        this._transformations[entity] = ExtraMath.TransformationMatrix(transform);
        this._cache[entity] = new(transform);


        return this._transformations[entity];
    }
}