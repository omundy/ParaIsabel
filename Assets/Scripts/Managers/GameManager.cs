using UnityEngine;

/// <summary>
/// Persistent root object that survives scene loads. The managers on its
/// children (SoundManager, DialogueManager, SceneChanger, ...) do the actual work.
/// </summary>
//
// Singleton pattern based on:
// https://gamedevbeginner.com/singletons-in-unity-the-right-way/
//
// HOW TO USE IT
//
// 1. Drag the GameManager prefab (Assets/Prefabs) into EVERY scene,
//    so you can press Play from any scene. Extra copies delete themselves.
//
// 2. Edit the PREFAB (double-click it in Assets/Prefabs), not the copy in a
//    scene. A scene's copy is deleted when you arrive from another scene,
//    so changes made to it are lost.
//
// 3. To add a manager, make an empty child object in the prefab (like
//    SoundManager) and add your script. Set it up in OnEnable or Start, not
//    Awake, so a copy that is about to be deleted doesn't run first.
//
// 4. Reach the managers from any script through Services, e.g.
//    Services.Get<SceneChanger>().OnClickNextScene();
//

// Run Awake before our other scripts, so a duplicate is deleted before its
// children start up. (Unity's Event System still starts first, causing a
// harmless "There can be only one active Event System" warning.)
[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    // The one GameManager, accessible from any script
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // A GameManager already exists, so this is a duplicate: delete it and its children.
        // DestroyImmediate (not Destroy) so the children can't run Start this frame.
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        // First load: become the one GameManager and survive scene loads
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Debug.Log($"*** GameManager (Singleton) created ***");
    }
}
