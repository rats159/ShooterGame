using System.Security.AccessControl;
using OpenTK.Mathematics;
using Shooter.Structures;

namespace Shooter.ECS.Components;

public class Transform(int x, int y, int width, int height, float degrees) : IComponent
{
    public Transform(Transform src) : this(src.X, src.Y, src.Width, src.Height, src.Degrees)
    {
    }

    public Transform(Box box) : this(box.X, box.Y, box.W, box.H, 0)
    {
    }

    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
    public float Degrees { get; set; } = degrees;

    public void Deconstruct(out int x, out int y, out int width, out int height, out float degrees)
    {
        x = this.X;
        y = this.Y;
        width = this.Width;
        height = this.Height;
        degrees = this.Degrees;
    }

    public void GetTransformedPoints(ref Span<Vector2> data)
    {
        float sin = MathF.Sin(MathHelper.DegreesToRadians(this.Degrees));
        float cos = MathF.Cos(MathHelper.DegreesToRadians(this.Degrees));

        float x = this.X;
        float y = this.Y;
        float w = this.Width;
        float h = this.Height;

        data[0] = (x, y);
        data[1] = (cos * w + x, sin * w + y);
        data[2] = (-sin * h + x, cos * h + y);
        data[3] = (cos * w - sin * h + x, sin * w + cos * h + y);
    }

    private bool Equals(Transform other)
    {
        return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height &&
               this.Degrees.Equals(other.Degrees);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return this.Equals((Transform)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.X, this.Y, this.Width, this.Height, this.Degrees);
    }

    public static bool operator !=(Transform self, Transform other)
    {
        return !(self == other);
    }

    public static bool operator ==(Transform self, Transform other)
    {
        if (self.X != other.X) return false;
        if (self.Y != other.Y) return false;
        if (self.Width != other.Width) return false;
        if (self.Height != other.Height) return false;
        if (!self.Degrees.Equals(other.Degrees)) return false;

        return true;
    }
}