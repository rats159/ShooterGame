using System.Collections;
using Shooter.Utility;

namespace Shooter.ECS;

public static class EntityManager
{
    private static ushort _id = 0;
    private static readonly Dictionary<Entity, List<IComponent>> _entityMap = [];

    public static List<List<IComponent>> Components => EntityManager._entityMap.Values.ToList();
    public static List<Entity> Entities => EntityManager._entityMap.Keys.ToList();

    public static int EntityCount => EntityManager._entityMap.Count;
    
    public static Entity New()
    {
        Entity ent = new(EntityManager._id++);
        EntityManager._entityMap[ent] = [];
        return ent;
    }

    public static void AddComponent(Entity entity, IComponent component)
    {
        EntityManager._entityMap[entity].Add(component);
    }
    
    public static List<Entity> GetWithComponent<T1>() where T1 : IComponent
    {
        List<Entity> ents = [];

        foreach ((Entity entity, List<IComponent> components) in EntityManager._entityMap)
        {
            if (components.OfType<T1>().Any())
            {
                ents.Add(entity);
            }
        }

        return ents;
    }

    public static List<Entity> GetWithComponents(params Type[] componentTypes)
    {
        List<Entity> ents = [];

        // Premature optimization? Maybe!
        Dictionary<Type, bool> typesFound = [];
        typesFound.EnsureCapacity(componentTypes.Length);
        
        foreach ((Entity ent, List<IComponent> components) in EntityManager._entityMap)
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
                ents.Add(ent);
            }
        }

        return ents;
    }

    public static List<IComponent> GetComponents(Entity ent)
    {
        return EntityManager._entityMap[ent];
    }
    
    public static List<T> GetComponents<T>() where T : IComponent
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

    public static T GetComponent<T>(Entity ent) where T : IComponent
    {
        foreach (IComponent component in EntityManager._entityMap[ent])
        {
            if (component is T tComp) return tComp;
        }

        throw new KeyNotFoundException($"Entity {ent} has no {typeof(T).Name} component");
    }

    public static T GetComponent<T>() where T : IComponent
    {
        foreach ((_, List<IComponent> list) in EntityManager._entityMap)
        {
            foreach (IComponent component in list)
            {
                if (component is T tComp) return tComp;
            }
        }

        throw new KeyNotFoundException($"No entities have component {typeof(T).Name}");
    }

    public static TypeMap<IComponent> GetComponentsDict(Entity ent)
    {
        TypeMap<IComponent> dict = new();
        foreach (IComponent comp in EntityManager._entityMap[ent])
        {
            dict.Set(comp);
        }

        return dict;
    }

    public static bool HasComponent<T>(Entity ent) where T : IComponent
    {
        foreach (IComponent component in EntityManager._entityMap[ent])
        {
            if (component is T) return true;
        }

        return false;
    }

    public static bool ComponentExists<T>() where T : IComponent
    {
        foreach (List<IComponent> compList in EntityManager.Components)
        {
            foreach (IComponent comp in compList)
            {
                if(comp is T) return true;
            }
        }

        return false;
    }
}