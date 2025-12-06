using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


public class GameManager : MonoBehaviour
{
    // https://gamedevbeginner.com/singletons-in-unity-the-right-way/
    public static GameManager Instance { get; private set; }
    public BuiltinLocalisedLineProvider LineProvider;
    public string localCode = "en";

    [Header("Scenes")]

    public int prevSceneIndex;
    public int currentSceneIndex;
    public int nextSceneIndex;

    void Awake()
    {
        // *** SINGLETON => If instance exists ...
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.activeSceneChanged += SceneChanged;
    }

    void SceneChanged(Scene _prevScene, Scene _newScene)
    {
        // 
    }


    public void OnClickEnglishButton() => ChangeLanguage("en");
    public void OnClickSpanishButton() => ChangeLanguage("es");
    public void ChangeLanguage(string _lang)
    {
        LineProvider.LocaleCode = _lang;
        Debug.Log($"LineProvider.LocaleCode={LineProvider.LocaleCode}");
    }





    void UpdateSceneInfo()
    {
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
