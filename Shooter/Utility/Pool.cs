namespace Shooter.Utility;

public class Pool<T> where T : notnull
{
    private readonly Dictionary<T, bool> pool = [];
    private readonly Func<T> _factory;
    
    public Pool(Func<T> factory,int capacity)
    {
        this._factory = factory;
        for (int i = 0; i < capacity; i++)
        {
            this.Expand();
        }
    }
    
    public T GetAndBusy()
    {
        foreach ((T obj, bool isBusy) in this.pool)
        {
            if (isBusy) continue;
            
            this.pool[obj] = true;
            return obj;
        }

        T newObj = this.Expand();
        this.pool[newObj] = true;
        return newObj;
    }

    public void Unbusy(T obj)
    {
        if (!this.pool.ContainsKey(obj))
        {
            throw new KeyNotFoundException($"Object {obj} not in pool!");
        }

        this.pool[obj] = false;
    }

    private T Expand()
    {
        T obj = this._factory();
        this.pool[obj] = false;
        return obj;
    }
}