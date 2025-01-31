using Shooter.Assets;
using Shooter.ECS;
using Shooter.ECS.Components;
using Shooter.Render.Quads;
using Shooter.Structures;

namespace Shooter.Levels;

public class Level
{
    private readonly List<Box> _structure;

    private Level(List<Box> structure)
    {
        this._structure = structure;
    }

    public static Level Save(List<Box> structure)
    {
        return new Level(structure);
    }

    // FIXME: this method basically leaks memory by not returning any created entities
    public static void Load(Level level)
    {
        foreach (Box box in level._structure)
        {
            ushort ent = EntityManager.New();
            EntityManager.AddComponent(ent, new TransformComponent(box));
            EntityManager.AddComponent(ent, new TextureComponent(AssetManager.GetTexture("level")));
            EntityManager.AddComponent(ent, new LevelRenderComponent(LevelQuad.Common));
        }
    }
}