namespace Shooter.Utility;

// For some reason c#'s Type class isn't generic, so this covers up the ugly casts.
public class TypeMap<T>
{
    private readonly Dictionary<Type, T> _underlying = [];

    public void Set(T value)
    {
        if (value == null) return; 
        this._underlying[value.GetType()] = value;
    }
    
    public T2 Get<T2>() where T2 : T
    {
        if (this._underlying.TryGetValue(typeof(T2), out T? value))
        {
            return (T2)value!;
        }

        throw new KeyNotFoundException();
    }
}