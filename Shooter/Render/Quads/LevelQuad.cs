using Shooter.Assets;

namespace Shooter.Render.Quads;

public class LevelQuad : PosQuad
{
    private static LevelQuad? _common;
    public static LevelQuad Common
    {
        get
        {
            if (LevelQuad._common != null) return LevelQuad._common;
            
            if (!AssetManager.Loaded)
            {
                throw new("LevelQuad.Common attempted to be read before window loaded.");
            }

            LevelQuad._common = new();

            return LevelQuad._common;
        }
    }
}
