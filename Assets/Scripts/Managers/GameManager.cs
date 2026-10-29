using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
/// Persistent manager that survives scene loads. Handles scene navigation,
/// starts the Yarn dialogue for each scene, and switches the dialogue language.
/// </summary>
public class GameManager : MonoBehaviour
{
    /////////////////////////////////////////////////////
    //////////////////// SINGLETON //////////////////////
    /////////////////////////////////////////////////////
    // https://gamedevbeginner.com/singletons-in-unity-the-right-way/

    // *** SINGLETON => make instance accessible outside of class
    public static GameManager Instance { get; private set; }

    // *** SINGLETON => only create once
    public bool singletonCreated = false;

    [Header("Dialogue")]
    // Yarn Spinner component that runs the dialogue nodes
    public DialogueRunner dialogueRunner;

    // Provides translated lines; changing its LocaleCode switches the language
    public BuiltinLocalisedLineProvider LineProvider;

    // Default language code ("en" = English, "es" = Spanish)
    public string localCode = "en";

    [Header("Scenes")]
    // Build indexes (File > Build Profiles) of the previous, current, and next scenes
    public int prevSceneIndex;
    public int currentSceneIndex;
    public int nextSceneIndex;

    [Header("Current Scene")]
    // The "SceneInfo" GameObject in the current scene, and its script
    public GameObject sceneInfo;
    public SceneInfo sceneInfoScript;

    void CreateSingleton()
    {
        // *** SINGLETON => If instance exists ...
        if (Instance != null && Instance.singletonCreated)
        {
            // Remove this duplicate's children first so none of them run
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            // *** SINGLETON => Then delete the object and exit
            DestroyImmediate(this.gameObject);
            return;
        }
        // Another instance exists but isn't flagged as created yet, so just remove this component
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // *** SINGLETON => Only reach this point on the first load...
        singletonCreated = true;
        Instance = this;
        // Keep this GameObject (and its children) alive when new scenes load
        DontDestroyOnLoad(this.gameObject);

        Debug.Log($"*** GameManager (Singleton) created ***");
    }

    private void Awake()
    {
        // Awake runs before Start, so the singleton is ready before other scripts use it
        CreateSingleton();
    }

    void Start()
    {
        // Call SceneChanged() every time a new scene becomes active
        SceneManager.activeSceneChanged += SceneChanged;

        // Start in the default language set in the Inspector
        ChangeLanguage(localCode);

        // Set up the first scene, since activeSceneChanged doesn't fire for it
        UpdateSceneInfo();
        StartCoroutine(CheckForDialogueAfterSceneLoad());
    }

    void OnDestroy()
    {
        // Unsubscribe so the event doesn't call a destroyed object
        SceneManager.activeSceneChanged -= SceneChanged;
    }

    /////////////////////////////////////////////////////
    //////////////////// LANGUAGE ///////////////////////
    /////////////////////////////////////////////////////

    void OnEnable()
    {
        // Listen for the language button events (triggered elsewhere via EventManager)
        EventManager.StartListening("ChangeToEnglish", OnClickEnglishButton);
        EventManager.StartListening("ChangeToSpanish", OnClickSpanishButton);
    }

    void OnDisable()
    {
        // Always stop listening when disabled to avoid calls to a destroyed object
        EventManager.StopListening("ChangeToEnglish", OnClickEnglishButton);
        EventManager.StopListening("ChangeToSpanish", OnClickSpanishButton);
    }

    public void OnClickEnglishButton() => ChangeLanguage("en");

    public void OnClickSpanishButton() => ChangeLanguage("es");

    // Switch the language Yarn uses for dialogue lines
    public void ChangeLanguage(string _lang)
    {
        LineProvider.LocaleCode = _lang;
        Debug.Log($"LineProvider.LocaleCode={LineProvider.LocaleCode}");
    }

    /////////////////////////////////////////////////////
    ///////////////////// SCENES ////////////////////////
    /////////////////////////////////////////////////////

    // Runs every time the active scene changes (subscribed in Start)
    void SceneChanged(Scene _prevScene, Scene _newScene)
    {
        UpdateSceneInfo();
        StartCoroutine(CheckForDialogueAfterSceneLoad());
    }

    // Find the scene's SceneInfo object and start its dialogue node, if it has one
    IEnumerator CheckForDialogueAfterSceneLoad()
    {
        yield return null; // Wait one frame so the new scene's objects exist
        sceneInfo = GameObject.Find("SceneInfo");
        if (sceneInfo == null)
        {
            Debug.Log("No SceneInfo GameObject");
            yield break;
        }

        sceneInfoScript = sceneInfo.GetComponent<SceneInfo>();
        if (sceneInfoScript == null)
        {
            Debug.Log("No SceneInfo script");
            yield break;
        }

        // Only start dialogue if a start node name is set in the Inspector
        if (!string.IsNullOrEmpty(sceneInfoScript.startDialogueNode))
            dialogueRunner.StartDialogue(sceneInfoScript.startDialogueNode);
        else
            Debug.Log("No startDialogueNode set in SceneInfo");
    }

    // Stop any running dialogue and recalculate the prev/current/next scene indexes
    void UpdateSceneInfo()
    {
        dialogueRunner.Stop();

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
    void GoToScene(int _buildIndex)
    {
        UpdateSceneInfo();
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
}
