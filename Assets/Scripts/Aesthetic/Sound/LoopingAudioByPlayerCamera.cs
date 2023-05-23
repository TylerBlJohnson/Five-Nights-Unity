using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Plays audioSource on loop or at random if doRandomChance is set
//If louderCamera is set to outside_office or win_screen and doRandomChance is true, volume is set based on randomAndNotLouderVolume
//If louderCamera is set to office, louderCameraVolume will not be used.
//If doRandomChance is false and louderCamera is set to a viable camera, audioSource will loop indefinitely with volume set based on the 3 non-random-specific volume settings
public class LoopingAudioByPlayerCamera : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameController.CameraMonitor louderCamera;
    [SerializeField] private bool disableVolumeCap = false;
    [SerializeField] private float louderCameraVolume = 1.0f;
    [SerializeField] private float otherCameraVolume = 0.5f;
    [SerializeField] private float officeVolume = 0.25f;

    [SerializeField] private bool doRandomChance = false;
    [SerializeField] private float randomAndNotLouderVolume;
    [SerializeField] private int oneInThisChancePerSecond = 1000;
    [SerializeField] private bool doRandomVolume = false;
    [SerializeField] private float minRandomVolume = 0.0f;
    [SerializeField] private float maxRandomVolume = 1.0f;
    private float secondTimer;
    private float audioTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (minRandomVolume > maxRandomVolume) {
            Debug.LogWarning("Min volume is higher than max volume!");
            minRandomVolume = maxRandomVolume;
        }
        if (doRandomChance) {
            audioSource.Stop();
            audioSource.loop = false;
            if (!disableVolumeCap) {
                if (doRandomVolume) {
                    if (minRandomVolume > 1
                    || maxRandomVolume > 1) {
                        Debug.LogWarning("Volume too high!\nEnable 'Disable volume cap' to uncap volume.");
                        if (minRandomVolume > 1) {
                            minRandomVolume = 1;
                        }
                        if (maxRandomVolume > 1) {
                            maxRandomVolume = 1;
                        }
                    }
                }
                if (louderCameraVolume > 1
                    || otherCameraVolume > 1
                    || officeVolume > 1) {
                    Debug.LogWarning("Volume too high!\nEnable 'Disable volume cap' to uncap volume.");
                    if (louderCameraVolume > 1) {
                        louderCameraVolume = 1;
                    }
                    if (otherCameraVolume > 1) {
                        otherCameraVolume = 1;
                    }
                    if (officeVolume > 1) {
                        officeVolume = 1;
                    }
                } 
            }
        } else if (louderCamera == GameController.CameraMonitor.Outside_Office
        || louderCamera == GameController.CameraMonitor.Win_Screen) {
            Debug.LogError("LouderCamera cannot be set to outside office or win screen if doRandomChance is disabled.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (doRandomChance) {
            secondTimer += Time.deltaTime;

            if (secondTimer >= 1.0f) {
                secondTimer -= 1.0f;

                if (Random.Range(0, oneInThisChancePerSecond) == 0 && audioTimer == 0.0f && !TempData.playerWon && !TempData.dying) {
                    audioSource.Play();
                    audioTimer = audioSource.clip.length;

                    if (doRandomVolume) {
                        audioSource.volume = Random.Range(minRandomVolume, maxRandomVolume);
                    }
                    //Debug.Log("Playing audio");
                } else {
                    //Debug.Log("Not playing audio");
                }
            }

            if (audioTimer >= 0.0f) {
                audioTimer -= Time.deltaTime;
            } else {
                audioTimer = 0.0f;
            }
        }

        if (doRandomVolume) { //Volume is completely random between two values
            //Just here to prevent overriding volume with something else, volume is set when audio is played
        } else if (doRandomChance //Volume is set, only applicable if doRandomChance is enabled
        &&(louderCamera == GameController.CameraMonitor.Outside_Office
        || louderCamera == GameController.CameraMonitor.Win_Screen)) {
            audioSource.volume = randomAndNotLouderVolume;
        } else if (louderCamera == GameController.CameraMonitor.Office) { //No monitor is louder than any other monitor
            if (TempData.playerViewingCamera == GameController.CameraMonitor.Office) {
                audioSource.volume = officeVolume;
            } else {
                audioSource.volume = otherCameraVolume;
            }
        }
        else { //Specific monitor will play at a different volume than other monitors, and office will play at a different volume than any monitor
            if (TempData.playerViewingCamera == louderCamera) {
                audioSource.volume = louderCameraVolume;
            } else if (TempData.playerViewingCamera == GameController.CameraMonitor.Office) {
                audioSource.volume = officeVolume;
            } else {
                audioSource.volume = otherCameraVolume;
            }
        }
    }
}
