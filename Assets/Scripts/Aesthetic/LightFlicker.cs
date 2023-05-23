using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private int maxNum = 10; //From 0 inclusive to given value exclusive
    [SerializeField] private int highestPass = 1;
    private Animator animator;
    [SerializeField] AudioSource lightHum;
    [SerializeField] private bool isOffice = false;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        if (isOffice && Application.platform == RuntimePlatform.WebGLPlayer) {
            lightHum.Play();
            lightHum.Pause();
        }
    }

    void FixedUpdate()
    {
        animator.SetBool("LightFlicker", Random.Range(0, maxNum+1) <= highestPass);

        if (isOffice) {
            //Debug.Log(Application.platform == RuntimePlatform.WindowsEditor);
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                //On WebGL builds, ".Pause()" actually stops playback rather than pausing, causing it to rapidly *restart* rather than pause and continue
                //For this reason, the sound for the light will continue playing even when the light is in the process of flickering
                if (TempData.lightOn) {
                    lightHum.UnPause();
                } else {
                    lightHum.Pause();
                }
            } else {
                //The hum may rapidly pause/unpause only if the build is not WebGL
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Power On.Left Light") 
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Power On.Right Light")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Power On.Bonnie")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Power On.Chica")
                ) {
                    lightHum.Play();
                } else {
                    lightHum.Pause();
                }
            }
        }
    }
}
