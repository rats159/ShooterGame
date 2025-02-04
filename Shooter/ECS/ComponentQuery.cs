using System.Collections;

namespace Shooter.ECS;

public class ComponentQuery
{
    private BitArray _bits = new(EntityManager.EntityCount);

    public static ComponentQuery All()
    {
        ComponentQuery query = new ComponentQuery();
        query._bits.SetAll(true);
        return query;
    }

    public static ComponentQuery Of<T>() where T : IComponent
    {
        ComponentQuery query = new();
        query.With<T>();
        return query;
    }

    public ComponentQuery Or<T>() where T : IComponent => this.With<T>();
    
    public ComponentQuery With<T>() where T : IComponent
    {
        for (int i = 0; i < this._bits.Count; i++)
        {
            if (this._bits[i]) continue;
            List<IComponent> components = EntityManager.Components[i];
            for (int j = 0; j < components.Count; j++)
            {
                if (components[j] is T)
                {
                    this._bits[i] = true;
                    break;
                }
            }
        }

        return this;
    }
    
    public ComponentQuery And<T>() where T : IComponent
    {
        for (int i = 0; i < this._bits.Count; i++)
        {
            if (!this._bits[i]) continue;
            List<IComponent> components = EntityManager.Components[i];
            bool found = false;
            for (int j = 0; j < components.Count; j++)
            {
                if (components[j] is T)
                {
                    found = true;
                    break;
                }
            }

            this._bits[i] = found;
        }

        return this;
    }

    public ComponentQuery Not<T>() where T : IComponent => this.Except<T>();
    public ComponentQuery Without<T>() where T : IComponent => this.Except<T>();
    
    public ComponentQuery Except<T>() where T : IComponent
    {
        for (int i = 0; i < this._bits.Count; i++)
        {
            if (!this._bits[i]) continue;
            List<IComponent> components = EntityManager.Components[i];
            for (int j = 0; j < components.Count; j++)
            {
                if (components[j] is T)
                {
                    this._bits[i] = false;
                    break;
                }
            }
        }

        return this;
    }

    public List<ushort> GetIds()
    {
        List<ushort> validIds = [];
        List<Entity> ents = EntityManager.Entities;
        
        for (int i = 0; i < this._bits.Count; i++)
        {
            if(!this._bits[i]) continue;
            validIds.Add(ents[i].Id);
        }

        return validIds;
    }
    
    public List<Entity> GetEntities()
    {
        List<Entity> validIds = [];
        List<Entity> allIds = EntityManager.Entities;
        
        for (int i = 0; i < this._bits.Count; i++)
        {
            if(!this._bits[i]) continue;
            validIds.Add(allIds[i]);
        }

        return validIds;
    }
    
    public List<T> Get<T>() where T : IComponent
    {
        List<T> found = [];
        List<Entity> allIds = EntityManager.Entities;
        
        for (int i = 0; i < this._bits.Count; i++)
        {
            if(!this._bits[i]) continue;
            if (allIds[i].Has<T>())
            {
                found.Add(allIds[i].Get<T>());
            }
        }

        return found;
    }
}