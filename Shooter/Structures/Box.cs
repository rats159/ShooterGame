namespace Shooter.Structures;

public readonly record struct Box(int X, int Y, int W, int H)
{
    public static implicit operator Box ((int x,int y,int w, int h) from)
    {
        return new(from.x,from.y,from.w,from.h);
    }
}