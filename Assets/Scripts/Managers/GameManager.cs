using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;


public class GameManager : MonoBehaviour
{
    // https://gamedevbeginner.com/singletons-in-unity-the-right-way/
    public static GameManager Instance { get; private set; }
    public BuiltinLocalisedLineProvider LineProvider;

    void Awake()
    {
        // *** SINGLETON => If instance exists ...
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        SceneManager.activeSceneChanged+=SceneChanged;

    }

    void SceneChanged(Scene A0, Scene A1)
    {
        // LineProvider.LocaleCode="es";

    }

}
