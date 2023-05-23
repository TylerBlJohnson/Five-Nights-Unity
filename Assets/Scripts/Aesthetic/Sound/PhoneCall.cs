using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCall : MonoBehaviour
{
    [SerializeField] private AudioSource[] phoneCall;
    [SerializeField] private GameObject muteButton;
    private float phoneCallTimer = 0.0f;
    private float showMuteButtonAfterSeconds = 20.0f;
    private float hideMuteButtonAfterSeconds;

    private int night;

    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        night = TempData.loadNight;

        if (night == 0) { //For debugging purposes
            night = 1;
        }

        night--; //loadNight starts at 1, the phoneCall[] array starts at 0

        muteButton.SetActive(false);
        if (night >= 0 && night < phoneCall.Length) {
            phoneCall[night].Play();
            hideMuteButtonAfterSeconds = phoneCall[night].clip.length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (phoneCallTimer < hideMuteButtonAfterSeconds) {
            phoneCallTimer += Time.deltaTime;

            if (showMuteButtonAfterSeconds < phoneCallTimer && !muteButton.activeSelf) {
                muteButton.SetActive(true);
            }

            if (TempData.playerViewingCamera == GameController.CameraMonitor.Office) {
                phoneCall[night].volume = 1.0f;
            } else {
                phoneCall[night].volume = 0.5f;
            }
        } else if (night >= 0 && night < phoneCall.Length){
            phoneCall[night].Stop();
            muteButton.SetActive(false);
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the mouse is pointing at something
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Debug.Log("Clicked " + hit.collider.name);
            if (hit.collider.name == muteButton.name) {
                phoneCallTimer = hideMuteButtonAfterSeconds;
            }
        }

        //Stop the phone call if the player dies (or if the player won, which is only possible while debugging with the timer ticking exceedingly quickly)
        if (TempData.dying || TempData.playerWon) {
            phoneCallTimer = hideMuteButtonAfterSeconds;
        }
    }
}
