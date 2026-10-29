using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/// <summary>
/// Starts each scene's Yarn dialogue (the node named in its SceneInfo)
/// and switches the dialogue language.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    // Yarn Spinner component that runs the dialogue nodes
    public DialogueRunner dialogueRunner;

    // Provides translated lines; changing its LocaleCode switches the language
    public BuiltinLocalisedLineProvider LineProvider;

    // Starting language code ("en" = English, "es" = Spanish)
    public string localeCode = "en";

    [Header("Current Scene")]
    // The "SceneInfo" GameObject in the current scene, and its script
    public GameObject sceneInfo;
    public SceneInfo sceneInfoScript;

    // The running dialogue check, so only one can run at a time
    Coroutine checkForDialogue;

    void OnEnable()
    {
        // Let other scripts find this with Services.Get<DialogueManager>()
        Services.Register(this);

        // Start the new scene's dialogue every time a new scene becomes active
        SceneManager.activeSceneChanged += SceneChanged;

        // Listen for the language button events (triggered elsewhere via EventManager)
        EventManager.StartListening("ChangeToEnglish", OnClickEnglishButton);
        EventManager.StartListening("ChangeToSpanish", OnClickSpanishButton);
    }

    void OnDisable()
    {
        // Always unsubscribe when disabled to avoid calls to a destroyed object
        Services.Unregister(this);
        SceneManager.activeSceneChanged -= SceneChanged;
        EventManager.StopListening("ChangeToEnglish", OnClickEnglishButton);
        EventManager.StopListening("ChangeToSpanish", OnClickSpanishButton);
    }

    void Start()
    {
        // Start in the language set in the Inspector
        ChangeLanguage(localeCode);

        // Start the first scene's dialogue (activeSceneChanged may or may not fire for it)
        CheckForDialogue();
    }

    /////////////////////////////////////////////////////
    //////////////////// LANGUAGE ///////////////////////
    /////////////////////////////////////////////////////

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

    // Stop the old scene's dialogue and start the new one's
    void SceneChanged(Scene _prevScene, Scene _newScene)
    {
        dialogueRunner.Stop();
        CheckForDialogue();
    }

    // Restart the dialogue check, cancelling one that is already waiting
    void CheckForDialogue()
    {
        if (checkForDialogue != null)
            StopCoroutine(checkForDialogue);
        checkForDialogue = StartCoroutine(CheckForDialogueAfterSceneLoad());
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
}
