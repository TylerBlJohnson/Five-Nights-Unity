using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the playing of the robotvoice audio.
//Audio plays at a volume depending on what's happening.
//Hallucination Overlay = full volume
//Bonnie/Chica seen glitching in the corner = slightly less volume
//Bonnie/Chica glitching in corner but not seen = very little volume
//None of the above = 0
public class RobotVoice : MonoBehaviour
{
    [SerializeField] private AudioSource robotVoice;

    private GameController.CameraMonitor bonnieLoc;
    private GameController.CameraMonitor chicaLoc;

    private GameController.CameraMonitor WHallCorner = GameController.CameraMonitor.West_Hall_Corner;
    private GameController.CameraMonitor EHallCorner = GameController.CameraMonitor.East_Hall_Corner;

    void Awake()
    {
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHICA_MOVED, OnChicaMoved);
    }

    void OnDestroy()
    {
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHICA_MOVED, OnChicaMoved);
    }

    // Update is called once per frame
    void Update()
    {
        if (TempData.playerWon || TempData.dying) {
            robotVoice.volume = 0.0f;
        } else if (TempData.hallucinationOverlayPlaying || TempData.manuallyHallucinate) { //Hallucination overlay playing
            //Debug.Log("Max volume");
            robotVoice.volume = 1.0f;
        } else if ((TempData.loadNight >= 4 || TempData.loadNight == 0) && bonnieLoc == WHallCorner && TempData.playerViewingCamera == WHallCorner) { //Player viewing Bonnie glitching
            //Debug.Log("Bonnie");
            robotVoice.volume = 0.7f;
        } else if ((TempData.loadNight >= 4 || TempData.loadNight == 0) && chicaLoc == EHallCorner && TempData.playerViewingCamera == EHallCorner) { //Player viewing Chica glitching
            //Debug.Log("Chica");
            robotVoice.volume = 0.7f;
        } else if ((TempData.loadNight >= 4 || TempData.loadNight == 0) && (bonnieLoc == WHallCorner || chicaLoc == EHallCorner)) { //Bonnie or Chica glitching but not seen
            //Debug.Log("Distant glitching");
            robotVoice.volume = 0.10f;
        } else {
            //Debug.Log("Muted");
            robotVoice.volume = 0.0f;
        }
    }

    void OnBonnieMoved(GameController.CameraMonitor monitor) 
    {
        bonnieLoc = monitor;
    }

    void OnChicaMoved(GameController.CameraMonitor monitor)
    {
        chicaLoc = monitor;
    }
}
