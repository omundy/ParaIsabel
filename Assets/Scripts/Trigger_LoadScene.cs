
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger_LoadScene : MonoBehaviour
{

    [SerializeField] BoxCollider2D boxCollider2D;
    public string sceneToLoad;

    void OnValidate()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    
            SceneManager.LoadScene(sceneToLoad);
       
    }


}