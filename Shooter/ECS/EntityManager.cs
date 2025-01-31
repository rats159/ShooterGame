using Shooter.Utility;

namespace Shooter.ECS;

public static class EntityManager
{
    private static ushort _id = 0;
    private static readonly Dictionary<ushort, List<IComponent>> _entityMap = [];

    public static ushort New()
    {
        ushort id = EntityManager._id++;
        EntityManager._entityMap[id] = [];
        return id;
    }

    public static void AddComponent(ushort id, IComponent component)
    {
        EntityManager._entityMap[id].Add(component);
    }
    
    public static List<ushort> GetWithComponent<T1>() where T1 : IComponent
    {
        List<ushort> ids = [];

        foreach ((ushort id, List<IComponent> components) in EntityManager._entityMap)
        {
            if (components.OfType<T1>().Any())
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    public static List<ushort> GetWithComponents(params Type[] componentTypes)
    {
        List<ushort> ids = [];

        // Premature optimization? Maybe!
        Dictionary<Type, bool> typesFound = [];
        typesFound.EnsureCapacity(componentTypes.Length);
        
        foreach ((ushort id, List<IComponent> components) in EntityManager._entityMap)
        {
            typesFound.Clear();
            foreach (Type t in componentTypes)
            {
                typesFound[t] = false;
            }


            foreach (IComponent component in components)
            {
                if (typesFound.ContainsKey(component.GetType()))
                {
                    typesFound[component.GetType()] = true;
                }
            }

            bool good = true;
            foreach ((_, bool found) in typesFound)
            {
                if (!found)
                {
                    good = false;
                    break;
                }
            }

            if (good)
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    public static List<IComponent> GetComponents(ushort id)
    {
        return EntityManager._entityMap[id];
    }
    
    public static List<T> GetComponents<T>() where T : class,IComponent
    {
        List<T> components = [];
        foreach ((_,List<IComponent> cmps) in EntityManager._entityMap)
        {
            foreach (IComponent component in cmps)
            {
                if (component is T tComp)
                {
                    components.Add(tComp);
                    break;
                }
            }
        }
        return components;
    }

    public static T? GetComponent<T>(ushort id) where T : class, IComponent
    {
        foreach (IComponent component in EntityManager._entityMap[id])
        {
            if (component is T tComp) return tComp;
        }

        return null;
    }

    public static T? GetComponent<T>() where T : class, IComponent
    {
        foreach ((_, List<IComponent> list) in EntityManager._entityMap)
        {
            foreach (IComponent component in list)
            {
                if (component is T tComp) return tComp;
            }
        }

        return null;
    }

    public static TypeMap<IComponent> GetComponentsDict(ushort id)
    {
        TypeMap<IComponent> dict = new();
        foreach (IComponent comp in EntityManager._entityMap[id])
        {
            dict.Set(comp);
        }

        return dict;
    }
}