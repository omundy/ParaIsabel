using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public  class YarnCommands : MonoBehaviour
{


    [YarnCommand("GoToScene")]
    public static void GoToScene(string sceneName) {
        Debug.Log($"GoToScene {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

}
