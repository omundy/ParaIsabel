using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public List<SceneMusic> sceneMusicList;

    void Start()
    {
        SetMusic();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next) => SetMusic();

    public void SetMusic()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is null!");
            return;
        }
        foreach (SceneMusic sceneMusic in sceneMusicList)
        {
            Debug.Log($"SetMusic() sceneMusic.startPlaying={sceneMusic.startPlaying}, scene.name={scene.name}");
            if (sceneMusic.startPlaying == scene.name)
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
                audioSource.clip = sceneMusic.clip;
                audioSource.Play();
            }
        }
    }
}

[Serializable]
public class SceneMusic
{
    public AudioClip clip;
    public string startPlaying;
}