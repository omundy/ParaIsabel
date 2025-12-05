using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace SneakawayUtilities
{
    /// <summary>
    /// Sets this, another gameObject, or a component active/inactive on event.
    /// Good for hiding prefabs, markers, popup UIs, etc. when not in use.
    /// </summary>
    public class PlayDialogueOnEvent : MonoBehaviour
    {
        public enum OnEvent { Awake, Enable, Start, Collision, Trigger, Time };
        [Tooltip("Event to act on")]
        public OnEvent onEvent;

        [Tooltip("Object to enable / disable")]
        public GameObject obj;

        [Tooltip("Component to enable / disable")]
        public Behaviour component;
        public SpriteRenderer spriteRenderer;
        public MeshRenderer meshRenderer;

        [Tooltip("Tag for collision and trigger events")]
        public string collisionTag; // e.g. "Player"

        [Tooltip("State to set")]
        public bool enable = true;

        [Tooltip("Has it been triggered?")]
        public bool triggered;


        public string nodename;
        public DialogueRunner dialogueRunner;




        ////////////////////////////////////////////////////// 
        ///////////////////// EVENTS /////////////////////////
        //////////////////////////////////////////////////////

        void Awake() => SetEnabled(OnEvent.Awake);
        void OnEnable() => SetEnabled(OnEvent.Enable);
        void Start() => SetEnabled(OnEvent.Start);

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log(collision.transform.tag);
            // NOTE: If using tags for collision checking etc. only add the tag to one GameObject in a scene.
            // Do not add the tag to its children as well, or you will be getting references to the wrong gameobjects!
            if (collision.transform.parent.CompareTag(collisionTag))
                SetEnabled(OnEvent.Collision);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log(collision.transform.tag);
            if (collision.transform.parent.CompareTag(collisionTag))
               dialogueRunner.StartDialogue(nodename);
        }

        void SetEnabled(OnEvent _onEvent)
        {
            if (onEvent == _onEvent && !triggered)
            {
                if (obj != null)
                    obj.SetActive(enable);
                if (component != null)
                    component.enabled = enable;
                if (spriteRenderer != null)
                    spriteRenderer.GetComponent<Renderer>().enabled = enable;
                if (meshRenderer != null)
                    meshRenderer.GetComponent<Renderer>().enabled = enable;

                triggered = true;
            }
        }


    }
}