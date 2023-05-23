using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallucinationOverlay : MonoBehaviour
{
    //Leader hallucination overlay sets random values in TempData which all overlays (including the leader) use to apply visibility and animation
    [SerializeField] private bool doSet = false;

    //Set in Start()
    private SpriteRenderer rend;
    private Color visible;
    private Color invisible;
    private Animator anim;

    private float secondTimer = 0.0f;
    private float tickTimer = 0.0f;

    //A (starts hallucination timer)
    //Rolls a random number from AMin to AMax (not including AMax) every second
    //If = 0, C starts counting down (hallucinations start playing)
    private static int AMin = 0;
    private static int AMax = 1000;

    //B (determines visibility)
    //Every tick (60 ticks per second) if C > 0, rolls a random number from BMin to BMax (not including BMax)
    //If = 0, hallucination is visible, else invisible
    private static int BMin = 0;
    private static int BMax = 10;

    //C (timer)
    //A may cause C to start counting down
    private float hallucinationTimer = 0.0f;
    private static float hallucinationEnd = 100.0f / 60.0f; //100 ticks at 60 ticks per second

    private static int numberOfHallucinations = 4;

    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<SpriteRenderer>();
        visible = new Color(1, 1, 1, 1);
        invisible = new Color(1, 1, 1, 0);
        anim = this.GetComponent<Animator>();

        rend.material.color = invisible;
    }

    // Update is called once per frame
    void Update()
    {
        if (doSet) {
            secondTimer += Time.deltaTime;
            tickTimer += Time.deltaTime;

            //Every second
            if (secondTimer >= 1.0f) {
                secondTimer -= 1.0f;

                if (Random.Range(AMin, AMax) == 0) {
                    hallucinationTimer = hallucinationEnd;
                }
            }

            //Every tick
            if (tickTimer >= 1.0f / 60.0f) {
                tickTimer -= 1.0f / 60.0f;

                if (Random.Range(BMin, BMax) == 0 && (hallucinationTimer > 0.0f || TempData.manuallyHallucinate) && !TempData.dying && !TempData.playerWon) {
                    TempData.hallucinationOverlayVisible = true;
                    TempData.hallucinationOverlayValue = Random.Range(0, numberOfHallucinations);
                } else {
                    TempData.hallucinationOverlayVisible = false;
                }
            }

            //Every frame
            if (hallucinationTimer > 0.0f && !TempData.dying && !TempData.playerWon) {
                hallucinationTimer -= Time.deltaTime;
                TempData.hallucinationOverlayPlaying = true; //This is just for the sound that accompanies the overlay, controlled by the RobotVoice script because there are other times it plays
            } else {
                hallucinationTimer = 0.0f;
                TempData.hallucinationOverlayPlaying = false;
            }
        }

        if (TempData.hallucinationOverlayVisible) {
            rend.material.color = visible;
        } else {
            rend.material.color = invisible;
        }
        anim.SetInteger("Hallucination", TempData.hallucinationOverlayValue);
    }
}
