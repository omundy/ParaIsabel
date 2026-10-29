using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Moves between scenes in build order. Hook the OnClick methods up to UI buttons.
/// </summary>
//
// HOW TO USE IT AS A SLIDE SHOW
//
// 1. Make one scene per slide (e.g. Slide_1, Slide_2, Slide_3...).
//
// 2. Open File > Build Profiles > Scene List and drag the slide scenes in,
//    in the order you want them shown. The number next to each scene is its
//    build index: the first slide is 0, the second is 1, and so on.
//
// 3. Drag the GameManager prefab (Assets/Prefabs) into EVERY slide scene.
//    SceneChanger is on one of its children. Having it in every scene means
//    you can press Play from any slide. Extra copies delete themselves.
//
// 4. Make the Next and Previous buttons INSIDE the GameManager prefab
//    (open the prefab and look at TestingCanvas > Controls for examples):
//      - Select the button and find On Click () in the Inspector
//      - Click +, then drag the SceneChanger object into the empty slot
//      - Choose SceneChanger > OnClickNextScene (or OnClickPrevScene)
//    Don't put these buttons in a slide scene: they would point to that
//    scene's copy of GameManager, which gets deleted when you arrive from
//    another slide, and then the button would stop working.
//
// 5. To jump to one particular slide (e.g. a "Back to start" button),
//    choose SceneChanger > GoToScene (int) and type the slide's build index,
//    or SceneChanger > GoToSceneByName (string) and type the scene's name.
//
// Next does nothing on the last slide and Previous does nothing on the
// first, so the slide show stops at both ends ("No more scenes" in the Console).
//
public class SceneChanger : MonoBehaviour
{
    [Header("Scenes")]
    // Build indexes (File > Build Profiles) of the previous, current, and next scenes
    public int prevSceneIndex;
    public int currentSceneIndex;
    public int nextSceneIndex;

    void OnEnable()
    {
        // Let other scripts find this with Services.Get<SceneChanger>()
        Services.Register(this);

        // Recalculate the indexes every time a new scene becomes active
        SceneManager.activeSceneChanged += SceneChanged;
        UpdateSceneIndexes();
    }

    void OnDisable()
    {
        Services.Unregister(this);
        // Unsubscribe so the event doesn't call a destroyed object
        SceneManager.activeSceneChanged -= SceneChanged;
    }

    void SceneChanged(Scene _prevScene, Scene _newScene) => UpdateSceneIndexes();

    // Recalculate the prev/current/next scene indexes
    void UpdateSceneIndexes()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Clamp keeps the indexes between the first (0) and last scene
        int lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        prevSceneIndex = Mathf.Clamp(currentSceneIndex - 1, 0, lastSceneIndex);
        nextSceneIndex = Mathf.Clamp(currentSceneIndex + 1, 0, lastSceneIndex);
    }

    // Called by the UI prev/next buttons
    public void OnClickPrevScene() => GoToScene(prevSceneIndex);

    public void OnClickNextScene() => GoToScene(nextSceneIndex);

    // Load a scene by build index, but only if it exists and isn't the current scene
    // (prev/next are clamped, so they equal the current index at the first/last scene)
    public void GoToScene(int _buildIndex)
    {
        UpdateSceneIndexes();
        if (
            _buildIndex >= 0
            && _buildIndex < SceneManager.sceneCountInBuildSettings
            && _buildIndex != currentSceneIndex
        )
        {
            SceneManager.LoadScene(_buildIndex);
        }
        else
        {
            Debug.Log("No more scenes in build settings.");
        }
    }

    // Load a scene by name (it must be in the build settings)
    public void GoToSceneByName(string _sceneName) => SceneManager.LoadScene(_sceneName);
}
