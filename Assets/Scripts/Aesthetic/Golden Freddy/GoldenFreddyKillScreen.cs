using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles the Golden Freddy kill screen behavior.
//Very simple script that waits one second before closing the game.
public class GoldenFreddyKillScreen : MonoBehaviour
{
    private float timeBeforeClose = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GFreddyKill");
    }

    IEnumerator GFreddyKill() {
        //Wait one second
        for (float i = timeBeforeClose; i > 0; i -= Time.deltaTime) {
            yield return null;
        }
        
        if (Application.platform != RuntimePlatform.WebGLPlayer) {
            //Close game
            Application.Quit();
        }

        //Restart the game if it fails to quit (in the editor for example)
        SceneManager.LoadScene("Warning Message");
    }
}
