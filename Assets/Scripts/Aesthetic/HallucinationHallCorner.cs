using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls Bonnie/Chica's hall corner twitching behavior on nights 4+ (and night 0 for debugging purposes)
public class HallucinationHallCorner : MonoBehaviour
{
    private Animator anim;
    private float twitchTimer = 0.0f;
    private float twitchUpdateTimer = 5.0f / 100.0f;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        anim.SetInteger("HallucinationTwitch", 0);
    }
    
    void Update()
    {
        //The twitch rng is rerolled every time twitchTimer reaches or passes twitchUpdateTimer. 
        //At 5.0f / 100.0f this means it updates 20 times per second.
        twitchTimer += Time.deltaTime;
        if (twitchTimer >= twitchUpdateTimer) {
            twitchTimer -= twitchUpdateTimer;

            //The characters should only twitch on nights 4 and up.
            //For testing purposes they also twitch on night 0 aka the debug night.
            if (TempData.loadNight >= 4 || TempData.loadNight == 0) {
                anim.SetInteger("HallucinationTwitch", Random.Range(0, 10));
            }
        }
    }
}
