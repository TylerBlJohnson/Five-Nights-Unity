using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightText : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Animator swapAnim;
    [SerializeField] private AudioSource blipClip;
    [SerializeField] private GameObject container;
    private Renderer rend;

    float startFadeOut = 2.0f;
    float endAnimation = 4.33f;
    bool doStart = false;

    void Awake()
    {
        Messenger.AddListener(GameEvent.START_NIGHT, DoStartNight);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.START_NIGHT, DoStartNight);
        StopCoroutine("StartNight");
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        rend = gameObject.GetComponent<Renderer>();
    }

    private void DoStartNight()
    {
        try
        {
            StartCoroutine("StartNight");
        }
        catch (System.Exception)
        {
            
        }
    }

    IEnumerator StartNight()
    {
        if (!doStart) {
            doStart = true;
            AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach( AudioSource audioS in allAudioSources) {
                audioS.Stop();
            }

            blipClip.Play();

            try
            {
                anim.SetInteger("Night", TempData.loadNight);
            }
            catch (System.Exception)
            {
                anim = GetComponent<Animator>();
                anim.SetInteger("Night", TempData.loadNight);
                //Debug.Log("Oh no");
            }
            
            
            if (Random.Range(0, 2) == 0) {
                swapAnim.SetTrigger("CamChange1");
            } else {
                swapAnim.SetTrigger("CamChange2");
            }

            yield return new WaitForSeconds(startFadeOut);

            for (float i = endAnimation - startFadeOut; i > 0; i -= Time.deltaTime) {
                // set color with i as alpha
                rend.material.color = new Color(1, 1, 1, i / (endAnimation - startFadeOut));
                yield return null;
            }
            rend.material.color = new Color(1, 1, 1, 0);

            SceneManager.LoadScene("FNaF Night");
        }
    }
}
