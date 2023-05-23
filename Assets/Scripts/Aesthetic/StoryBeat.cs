using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryBeat : MonoBehaviour
{
    [SerializeField] private GameObject storyBeat;
    [SerializeField] private SpriteRenderer fadeIn;
    [SerializeField] private AudioSource musicBox;
    private SpriteRenderer texture;
    private Animator anim;
    private float timeFading = 1.5f;
    private float timeWaiting = 5.0f;
    private float timePaid = 17.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        //If the passed cutscene value is invalid, go back to main menu
        if (TempData.playStoryBeat < 1 || TempData.playStoryBeat > 4) {
            SceneManager.LoadScene("Main Menu");
        }

        //Get the cutscene object renderer and ensure it's visible
        texture = storyBeat.GetComponent<SpriteRenderer>();
        texture.material.color = new Color(1, 1, 1, 1);

        //Apply screenshot sprite if applicable, and ensure it's visible
        if (TempData.playStoryBeat == (int)MainMenu.StoryBeat.New_Game) { //Only the new game cutscene fades in from anything other than a black screen
            fadeIn.sprite = TempData.screenshot;
        } else { //Stop main menu audio and play music box instead
            AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach( AudioSource audioS in allAudioSources) {
                audioS.Stop();
            }
            musicBox.Play();
        }
        fadeIn.color = new Color(1, 1, 1, 1);

        //Apply the correct texture to the scene
        anim = storyBeat.GetComponent<Animator>();
        anim.SetInteger("StoryBeat", TempData.playStoryBeat);

        //Play the cutscene
        DoStoryBeat();
    }

    void Update()
    {
        //Upon clicking enter (return key) or left mouse button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Mouse0)) {
            //Skip the rest of the cutscene
            SceneManager.LoadScene("Main Menu");
        }
    }

    private void DoStoryBeat() 
    {
        StartCoroutine("StoryBeatCutscene");
    }

    IEnumerator StoryBeatCutscene() {
        //Fade the "fadeIn" object out
        for (float i = timeFading; i > 0; i -= Time.deltaTime) {
            // set color with i as alpha
            fadeIn.material.color = new Color(1, 1, 1, i / timeFading);
            yield return null;
        }
        fadeIn.material.color = new Color(1, 1, 1, 0);

        //Free up the memory being taken by the screenshot
        TempData.screenshot = default;

        //Wait however many seconds
        float waitTime;
        if (TempData.playStoryBeat == (int)MainMenu.StoryBeat.New_Game) {
            waitTime = timeWaiting;
        } else {
            waitTime = timePaid;
        }
        for (float i = 0.0f; i < waitTime; i += Time.deltaTime) {
            yield return null;
        }

        //Fade the scene out
        for (float i = timeFading; i > 0; i -= Time.deltaTime) {
            texture.material.color = new Color(1, 1, 1, i / timeFading);
            yield return null;
        }
        texture.material.color = new Color(1, 1, 1, 0);

        //Switch to the main menu
        SceneManager.LoadScene("Main Menu");
    }
}
