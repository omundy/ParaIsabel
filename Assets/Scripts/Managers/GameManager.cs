using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


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

    public DialogueRunner dialogueRunner;
    public BuiltinLocalisedLineProvider LineProvider;
    public string localCode = "en";
    public SceneInfo sceneInfo;


    [Header("Scenes")]

    public int prevSceneIndex;
    public int currentSceneIndex;
    public int nextSceneIndex;

    void CreateSingleton()
    {
        // *** SINGLETON => If instance exists ...
        if (Instance != null && Instance.singletonCreated)
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            // *** SINGLETON => Then delete the object and exit
            DestroyImmediate(this.gameObject);
            return;
        }
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // *** SINGLETON => Only reach this point on the first load...
        singletonCreated = true;
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        Debug.Log($"*** GameManager (Singleton) created ***");
    }

    private void Awake()
    {
        CreateSingleton();
        UpdateSceneInfo();
    }

    void Start() => SceneManager.activeSceneChanged += SceneChanged;



    void OnEnable()
    {
        EventManager.StartListening("ChangeToEnglish", OnClickEnglishButton);
        EventManager.StartListening("ChangeToSpanish", OnClickSpanishButton);
    }
    void OnDisable()
    {
        EventManager.StopListening("ChangeToEnglish", OnClickEnglishButton);
        EventManager.StopListening("ChangeToSpanish", OnClickSpanishButton);
    }



    public void OnClickEnglishButton() => ChangeLanguage("en");
    public void OnClickSpanishButton() => ChangeLanguage("es");
    public void ChangeLanguage(string _lang)
    {
        LineProvider.LocaleCode = _lang;
        Debug.Log($"LineProvider.LocaleCode={LineProvider.LocaleCode}");
    }





    void SceneChanged(Scene _prevScene, Scene _newScene)
    {
        UpdateSceneInfo();

        GameObject sceneInfoGo = GameObject.Find("SceneInfo");
        if (sceneInfo != null)
        {
            sceneInfo = sceneInfoGo.GetComponent<SceneInfo>();
            if (sceneInfo.startDialogueNode != "")
                dialogueRunner.StartDialogue(sceneInfo.startDialogueNode);
        }
    }

    void UpdateSceneInfo()
    {
        dialogueRunner.Stop();

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        prevSceneIndex = Mathf.Clamp(currentSceneIndex - 1, 0, SceneManager.sceneCountInBuildSettings);
        nextSceneIndex = Mathf.Clamp(currentSceneIndex + 1, 0, SceneManager.sceneCountInBuildSettings);

    }

    public void OnClickPrevScene() => GoToScene(currentSceneIndex - 1);
    public void OnClickNextScene() => GoToScene(nextSceneIndex);

    void GoToScene(int _buildIndex)
    {
        UpdateSceneInfo();
        if (_buildIndex >= 0 && _buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(_buildIndex);
        }
        else
        {
            Debug.Log("No more scenes in build settings.");
        }
    }

}
