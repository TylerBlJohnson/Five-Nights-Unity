using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    //The two "Power left:" digits as well as the battery usage meter
    [SerializeField] private Animator tensDigit;
    [SerializeField] private Animator onesDigit;
    [SerializeField] private Animator usageMeter;

    //Determines how often to drain power, this variable refers to the usage meter specifically.
    private static float drainPowerInSecondsByUsage = 1.0f;
    //Timer that ticks up to the above variable before resetting and draining power
    private float drainPowerByUsageTimer = 0.0f;

    //FNaF has a mechanic where 1 tick of power will be drained after a set period of time has passed, this is in addition to the normal power drainage via the "usage" meter
    //Variable is set in Start()
    private float drainPowerInSecondsByNight;
    //Timer that ticks up to the above variable before resetting and draining power
    private float drainPowerByNightTimer = 0.0f;

    //As there are several instances of the usage meter at once, only one of them should be draining power. The rest are just aesthetic
    [SerializeField] private bool letThisInstanceCalculate = false;

    // Start is called before the first frame update
    void Start()
    {
        if (letThisInstanceCalculate) {
            TempData.remainingPower = 999;
            switch (TempData.loadNight) { 
                case(1):
                    drainPowerInSecondsByNight = 999.9f; //A night should last about 540 seconds. On night 1 the passive power drainage mechanic is disabled.
                    break;
                case(2):
                    drainPowerInSecondsByNight = 6.0f;
                    break;
                case(3):
                    drainPowerInSecondsByNight = 5.0f;
                    break;
                case(4):
                    drainPowerInSecondsByNight = 4.0f;
                    break;
                default:
                    drainPowerInSecondsByNight = 3.0f;
                    break;
            }
        }

        UpdatePower();
    }

    // Update is called once per frame
    void Update()
    {
        //Ticks up the two power draining timers, then removes some remaining power if applicable
        //Also stops ticking if the player has won
        if (letThisInstanceCalculate && !TempData.playerWon) {
            drainPowerByUsageTimer += Time.deltaTime;
            drainPowerByNightTimer += Time.deltaTime;
            
            if (drainPowerByUsageTimer >= drainPowerInSecondsByUsage) {
                drainPowerByUsageTimer -= drainPowerInSecondsByUsage;
                TempData.remainingPower -= GetPowerUsage();
            }

            if (drainPowerByNightTimer >= drainPowerInSecondsByNight) {
                drainPowerByNightTimer -= drainPowerInSecondsByNight;
                TempData.remainingPower--;
            }

            if (TempData.remainingPower < 0) {
                TempData.remainingPower = 0;
            }
        }

        UpdatePower();
    }

    //Purely aesthetic, updates the various animators
    private void UpdatePower()
    {
        int displayedPower = TempData.remainingPower / 10;
        tensDigit.SetBool("Visible", displayedPower / 10 > 0);
        tensDigit.SetInteger("Digit", displayedPower / 10);
        onesDigit.SetInteger("Digit", displayedPower % 10);
        usageMeter.SetInteger("Usage", GetPowerUsage());
    }

    //Returns the value of the "Usage" meter, from 1-5 (5 is unused)
    private int GetPowerUsage() {
        int powerUsage = 1;

        if (TempData.lightOn 
        || TempData.playerViewingCamera != GameController.CameraMonitor.Office) { powerUsage++; }
        if (TempData.leftDoorDown) { powerUsage++; }
        if (TempData.rightDoorDown) { powerUsage++; }

        return powerUsage;
    }
}
