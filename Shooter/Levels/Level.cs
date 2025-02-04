using System.Runtime.InteropServices.JavaScript;
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
            Entity ent = EntityManager.New();
            ent.AddComponent(new Transform(box));
            ent.AddComponent(new TextureComponent(AssetManager.GetTexture("level")));
            ent.AddComponent(new LevelRenderComponent(LevelQuad.Common));
            ent.AddComponent(new VerletObject(box.X,box.Y,box.W,box.H,true));
        }
    }
}