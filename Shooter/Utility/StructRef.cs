namespace Shooter.Utility;

// This is specifically for vectors because having a reference is very useful but nearly impossible
public class StructRef<T>(T value) where T : struct
{
    private T _value = value;

    public ref T Value => ref this._value;
}