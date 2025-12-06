
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger_Event : MonoBehaviour
{

    public string eventName;
    
    public void Trigger()
    {
        EventManager.TriggerEvent(eventName);
    }



}