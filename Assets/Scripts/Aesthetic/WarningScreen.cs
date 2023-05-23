using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningScreen : MonoBehaviour
{
    [SerializeField] private SpriteRenderer text;
    private float timeFading = 1.5f;
    private float timeWaiting = 2.0f;

    /*
    FadeOutOnWin[j].GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, i);
    //*/

    // Start is called before the first frame update
    void Start()
    {
        text.material.color = new Color(1, 1, 1, 0);
        DoWarningMessage();
    }

    void Update()
    {
        //Upon clicking enter (return key) or left mouse button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0)) {
            //Skip the rest of the warning message
            SceneManager.LoadScene("Main Menu");
        }
    }

    private void DoWarningMessage() 
    {
        StartCoroutine("WarningIntro");
    }

    IEnumerator WarningIntro() {
        //Fade the text in
        for (float i = 0.0f; i < timeFading; i += Time.deltaTime) {
            // set color with i as alpha
            text.material.color = new Color(1, 1, 1, i / timeFading);
            yield return null;
        }
        text.material.color = new Color(1, 1, 1, 1);

        //Wait timeWaiting seconds
        for (float i = 0.0f; i < timeWaiting; i += Time.deltaTime) {
            yield return null;
        }

        //Fade the text out
        for (float i = timeFading; i > 0; i -= Time.deltaTime) {
            text.material.color = new Color(1, 1, 1, i / timeFading);
            yield return null;
        }

        //Switch to the main menu
        SceneManager.LoadScene("Main Menu");
    }
}
