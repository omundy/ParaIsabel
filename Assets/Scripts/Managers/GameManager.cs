using UnityEngine;

public class GameManager : MonoBehaviour
{
    // https://gamedevbeginner.com/singletons-in-unity-the-right-way/
    public static GameManager Instance { get; private set; }

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
    }

}
