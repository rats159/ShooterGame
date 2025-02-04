using OpenTK.Mathematics;
using Shooter.ECS.Components;
using Shooter.Structures;

namespace Shooter.Utility;

public static class ExtraMath
{
    public static Matrix4 TransformationMatrix(float x, float y, float sx, float sy, float degrees)
    {
        Matrix4 mat = Matrix4.CreateScale(sx, sy, 1);
        mat *= Matrix4.CreateRotationZ((degrees) * 0.01745329f);
        mat *= Matrix4.CreateTranslation(x, y, 0);
        return mat;
    }
    
    public static Matrix4 TransformationMatrix(Transform transform)
    {
        return ExtraMath.TransformationMatrix(transform.X, transform.Y, transform.Width, transform.Height, transform.Degrees);
    }

    public static void Rotate(ref Vector2 src, float degrees)
    {
        float sin = MathF.Sin(MathHelper.DegreesToRadians(degrees));
        float cos = MathF.Cos(MathHelper.DegreesToRadians(degrees));

        float x1 = src.X;
        float y1 = src.Y;
        
        src.X = cos * x1 - sin * y1;
        src.Y = sin * x1 + cos * y1;
    }
}