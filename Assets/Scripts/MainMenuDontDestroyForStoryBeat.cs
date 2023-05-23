using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDontDestroyForStoryBeat : MonoBehaviour
{
    int numScenes = 0;
    // called first
    void OnEnable()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Story Beat") {
            if (numScenes == 0) { //Allows this to exist upon loading initially
                numScenes++;
            } else { //Runs upon loading any scene besides the initial scene or the Story Beat scene
                Destroy(this.gameObject);
            }
        }
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
