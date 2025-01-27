namespace Shooter.Structures;

public readonly record struct Box(int X, int Y, int W, int H)
{
    public static implicit operator Box ((int,int,int,int) from)
    {
        return new(from.Item1,from.Item2,from.Item3,from.Item4);
    }
}