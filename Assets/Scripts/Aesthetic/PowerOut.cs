using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the behavior of the power shutting off
public class PowerOut : MonoBehaviour
{
    public enum PowerState 
    {
        Power_On,
        Power_Off,
        Freddy_Music,
        Flicker_Off,
        Await_Death,
        Jumpscare
    }

    private Animator anim;

    private float twentiethOfASecond = 1.0f / 20.0f;
    private float timer = 0.0f;
    private bool playSequence = false;

    [SerializeField] private GameObject[] disableOnPowerLoss;
    [SerializeField] private AudioSource powerDown;
    [SerializeField] private AudioSource freddyMusic;
    [SerializeField] private AudioSource ambience;
    [SerializeField] private AudioSource freddyStepping;
    [SerializeField] private AudioSource lightHum;


    // Start is called before the first frame update
    void Start()
    {
        TempData.powerState = PowerState.Power_On;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= twentiethOfASecond) {
            timer -= twentiethOfASecond;
            anim.SetBool("FreddyFlicker", Random.Range(0, 4) != 0);
            
            if (TempData.powerState == PowerState.Flicker_Off) {
                if (Random.Range(0, 2) == 0) {
                    anim.SetBool("PowerFlicker", true);
                    lightHum.Play();
                } else {
                    lightHum.Pause();
                }

            }
        }

        if (TempData.remainingPower <= 0 && !playSequence) {
            StartCoroutine("PowerOutageSequence");
        }

        if (TempData.playerWon) {
            StopCoroutine("PowerOutageSequence");
        }
    }

    private IEnumerator PowerOutageSequence()
    {
        float stateTimer = 0.0f;                //Used with stopTimerAt
        float miniStateTimer = 0.0f;            //Used with the other timers
        float tinyTimer = 1.0f / 3.0f;          //Used for the "Flicker_Off" state
        float shortTimer = 2.0f;                //Used for the wait between state "Await_Death" and "Jumpscare"
        float longTimer = 5.0f;                 //Used for the other waits between states
        float stopTimerAt = 20.0f;              //Forces state changes at certain points

        playSequence = true;
        TempData.dying = true;
        lightHum.Pause();
        foreach (GameObject gO in disableOnPowerLoss) {
            gO.SetActive(false);
        }
        TempData.leftDoorDown = false;
        TempData.rightDoorDown = false;

        //Turn power off
        TempData.powerState = PowerState.Power_Off;
        anim.SetInteger("PowerState", (int)TempData.powerState);
        powerDown.Play();
        Messenger.Broadcast(GameEvent.SWITCH_TO_OFFICE);

        //Wait for Freddy to start playing music
        int freddyWalkCycles = 1;
        for (int i = 0; i < 3; i++) {
            if (Random.Range(0, 5) == 0) {
                break;
            } else {
                freddyWalkCycles++;
            }
        }

        freddyStepping.PlayDelayed(3.0f);
        switch (freddyWalkCycles)
        {
            case(4):
                freddyStepping.volume = 0.1f;
                break;
            case(3):
                freddyStepping.volume = 0.2f;
                break;
            case(2):
                freddyStepping.volume = 0.3f;
                break;
            case(1): 
                freddyStepping.volume = 0.4f;
                break;
            default:
                Debug.LogError("Invalid number of Freddy walk cycles");
                break;
        }
        while (TempData.powerState == PowerState.Power_Off)
        {
            stateTimer += Time.deltaTime;
            miniStateTimer += Time.deltaTime;

            if (stateTimer >= stopTimerAt) {
                TempData.powerState = PowerState.Freddy_Music;
            }
            if (miniStateTimer >= longTimer) {
                miniStateTimer -= longTimer;

                if (freddyWalkCycles > 0) {
                    freddyWalkCycles--;
                    freddyStepping.volume += 0.1f;
                } else {
                    TempData.powerState = PowerState.Freddy_Music;
                }
            }
            yield return null;
        }
        stateTimer = 0.0f;
        miniStateTimer = 0.0f;
        anim.SetInteger("PowerState", (int)TempData.powerState);
        freddyStepping.Stop();
        freddyMusic.Play();

        //Wait for Freddy to stop playing music
        while (TempData.powerState == PowerState.Freddy_Music)
        {
            stateTimer += Time.deltaTime;
            miniStateTimer += Time.deltaTime;

            if (stateTimer >= stopTimerAt) {
                TempData.powerState = PowerState.Flicker_Off;
            }
            if (miniStateTimer >= longTimer) {
                miniStateTimer -= longTimer;

                if (Random.Range(0, 5) == 0) {
                    TempData.powerState = PowerState.Flicker_Off;
                }
            }
            yield return null;
        }
        stateTimer = 0.0f;
        miniStateTimer = 0.0f;
        anim.SetInteger("PowerState", (int)TempData.powerState);
        freddyMusic.Stop();

        //Flicker the lights off
        for (float i = 0.0f; i < tinyTimer; i += Time.deltaTime) {
            yield return null;
        }
        TempData.powerState = PowerState.Await_Death;
        anim.SetInteger("PowerState", (int)TempData.powerState);
        lightHum.Pause();
        freddyStepping.PlayDelayed(3.0f);

        //Wait for the jumpscare
        while (TempData.powerState == PowerState.Await_Death)
        {
            stateTimer += Time.deltaTime;
            miniStateTimer += Time.deltaTime;

            if (stateTimer >= stopTimerAt) {
                TempData.powerState = PowerState.Jumpscare;
            }
            if (miniStateTimer >= shortTimer) {
                miniStateTimer -= shortTimer;

                if (Random.Range(0, 5) == 0) {
                    TempData.powerState = PowerState.Jumpscare;
                }
            }
            yield return null;
        }
        
        //Jumpscare
        freddyStepping.Stop();
        anim.SetInteger("PowerState", (int)TempData.powerState);
    }
}
