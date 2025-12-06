using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


/// <summary>
/// Play a dialogue node on an event.
/// </summary>
public class PlayDialogueOnEvent : MonoBehaviour
{
    public enum OnEvent { Awake, Enable, Start, Collision, Trigger, Time };
    [Tooltip("Event to act on")]
    public OnEvent onEvent;

    [Tooltip("Tag for collision and trigger events")]
    public string collisionTag; // e.g. "Player"

    public string nodeToPlay;
    public DialogueRunner dialogueRunner;

    [Tooltip("How many times can the dialogue be played?")]
    public int playedMax = 1;
    public int played;


    ////////////////////////////////////////////////////// 
    ///////////////////// EVENTS /////////////////////////
    //////////////////////////////////////////////////////

    void OnValidate()
    {
        // if (dialogueRunner == null)
        //     Debug.LogError("A Dialogue System is required. Drag the Dialogue System object into the correct field to create a reference.");
        // if (nodeToPlay == "")
        //     Debug.LogError("Add a node name from your Yarn Script");
    }

    void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = GameObject.Find("GameManager").GetComponentInChildren<DialogueRunner>();
    }

    void OnEnable() => PlayDialogue(OnEvent.Enable);
    void Start() => PlayDialogue(OnEvent.Start);

    // NOTE: If using tags for collision checking etc. only add the tag to one GameObject in a scene.
    // Do not add the tag to its children as well, or you will be getting references to the wrong gameobjects!
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag(collisionTag)
            || (collision.transform.parent && collision.transform.parent.CompareTag(collisionTag)))
        {
            Debug.Log($"OnCollisionEnter2D() tag={collision.transform.tag}");
            PlayDialogue(OnEvent.Collision);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(collisionTag)
            || (collision.transform.parent && collision.transform.parent.CompareTag(collisionTag)))
        {
            Debug.Log($"OnTriggerEnter2D() tag={collision.transform.tag}");
            PlayDialogue(OnEvent.Trigger);
        }
    }

    void PlayDialogue(OnEvent _onEvent)
    {
        if (onEvent == _onEvent && played < playedMax)
        {
            Debug.Log($"PlayDialogue() nodeToPlay={nodeToPlay}");
            dialogueRunner.StartDialogue(nodeToPlay);
            played++;
        }
    }


}