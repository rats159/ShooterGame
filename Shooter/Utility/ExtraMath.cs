using OpenTK.Mathematics;
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
}