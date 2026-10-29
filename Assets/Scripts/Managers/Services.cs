using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service Locator: one place to find managers without dragging references
/// in the Inspector or searching the hierarchy by name.
/// </summary>
//
// HOW TO USE IT
//
// 1. From any script, ask for the manager you need by its type:
//      Services.Get<SceneChanger>().OnClickNextScene();
//      Services.Get<DialogueManager>().ChangeLanguage("es");
//    Get returns null (and logs a warning) if there is no such manager.
//
// 2. In your own managers, register in OnEnable and unregister in OnDisable:
//      void OnEnable() => Services.Register(this);
//      void OnDisable() => Services.Unregister(this);
//    This is optional: if a manager isn't registered, Get searches the
//    scene for it once and remembers it. Registering is just faster and clearer.
//
public static class Services
{
    // One manager per type, e.g. typeof(SceneChanger) => the SceneChanger component
    static readonly Dictionary<Type, Component> services = new Dictionary<Type, Component>();

    // Empty the list when Play starts, in case it is left over from last time
    // (it can be if Project Settings > Editor > Enter Play Mode Settings skips the domain reload)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset() => services.Clear();

    // Add a manager so other scripts can find it
    public static void Register<T>(T service)
        where T : Component
    {
        services[typeof(T)] = service;
    }

    // Remove a manager, but only if it is the one that was registered
    public static void Unregister<T>(T service)
        where T : Component
    {
        if (services.TryGetValue(typeof(T), out Component current) && current == service)
            services.Remove(typeof(T));
    }

    // Find a manager by type
    public static T Get<T>()
        where T : Component
    {
        // "service != null" is also false for a manager that has been destroyed
        if (services.TryGetValue(typeof(T), out Component service) && service != null)
            return (T)service;

        // Not registered (yet), e.g. asked for in another script's Awake: search the scene
        T found = UnityEngine.Object.FindFirstObjectByType<T>();
        if (found != null)
            services[typeof(T)] = found;
        else
            Debug.LogWarning($"Services: no {typeof(T).Name} found");
        return found;
    }
}
