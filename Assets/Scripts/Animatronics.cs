using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles the animatronics movement behavior.
//Foxy failure behavior is handled by GameController.cs
public class Animatronics : MonoBehaviour
{
    public enum Names {
        None,
        Bonnie, 
        Chica,
        Freddy,
        Foxy
    }

    public enum GoldenFreddyStates {
        Waiting,    //Hasn't appeared
        Poster,     //In the West Hall Corner
        Office,     //In the office
        Finished    //Has already attacked this night
    }

    [SerializeField] AudioSource[] freddyLaughAudio;
    [SerializeField] AudioSource freddySteppingAudio;
    [SerializeField] AudioSource freddyMusic;
    [SerializeField] AudioSource freddyOfficeAudio;
    [SerializeField] AudioSource bonnieSteppingAudio;
    [SerializeField] AudioSource chicaSteppingAudio;
    [SerializeField] AudioSource goldenFreddyGiggleAudio;
    [SerializeField] AudioSource[] chicaKitchenAudio;

    //Move timer is movement opportunity
    //Wait is how long the animatronic has waited since their last movement opportunity
    //Near camera is used for when the animatronic is randomly selected to be near the camera or not
    //Location is the animatronic's current location
    //Foxy penalty is how long Foxy must wait after the player stops looking at the camera before he's allowed to move again
    //Foxy prep is Foxy's equivalent to the location variable and is stored in TempData. (TempData.foxyPrep)

    private static float bonnieMoveTimer = 4.97f;
    private float bonnieWait;
    private bool bonnieNearCamera = false;
    private GameController.CameraMonitor bonnieLocation;
    private static float chicaMoveTimer = 4.98f;
    private float chicaWait;
    private bool chicaNearCamera = false;
    private GameController.CameraMonitor chicaLocation;
    private float chicaInKitchenSoundCountdown = 0.0f;
    private static float freddyMoveTimer = 3.02f;
    private float freddyWait;
    private float freddyDelay; //Freddy's movement is delayed by a length of time depending on his AI level
    private GameController.CameraMonitor freddyLocation;
    private static float foxyMoveTimer = 5.01f;
    private float foxyWait;
    private float foxyPenalty;
    private static float goldenFreddyMoveTimer = 1.0f;
    private int goldenFreddyChance = 100000; //Golden Freddy has a 1/X chance to appear every second.
    private float goldenFreddyWait;
    private GoldenFreddyStates goldenFreddyStatus;
    private static float goldenFreddyKillTimer = 5.0f;

    private int bonnieAI;
    private int chicaAI;
    private int freddyAI;
    private int foxyAI;
    private static int maxAI = 20;

    void Awake()
    {
        Messenger<int>.AddListener(GameEvent.TIME_CHANGE, OnTimeChanged);       //Used to bump up Bonnie/Chica/Foxy AI levels at specific points throughout the nights.
        Messenger.AddListener(GameEvent.SWITCH_TO_MONITOR, OnSwitchToMonitor);  //Lets the player protect themselves against Golden Freddy appearing in the office.
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.TIME_CHANGE, OnTimeChanged);
        Messenger.RemoveListener(GameEvent.SWITCH_TO_MONITOR, OnSwitchToMonitor);
    }

    // Start is called before the first frame update
    void Start()
    {
        Reset();
        StartNight();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimes();

        //Bonnie movement opportunity
        if (bonnieWait >= bonnieMoveTimer) {
            bonnieWait -= bonnieMoveTimer;

            //Bonnie will move at random based on his AI level
            if (Random.Range(1, maxAI + 1) <= bonnieAI && !TempData.playerWon && !TempData.dying) {

                GameController.CameraMonitor bonnieOldLoc = bonnieLocation;
                //Bonnie can move to one of two locations based on his current location
                if (Random.Range(0, 2) == 0) {
                    switch (bonnieLocation) {
                        case (GameController.CameraMonitor.Show_Stage):
                        case (GameController.CameraMonitor.Dining_Area):
                            bonnieLocation = GameController.CameraMonitor.Backstage;
                            break;
                        case (GameController.CameraMonitor.Backstage):
                            bonnieLocation = GameController.CameraMonitor.Dining_Area;
                            break;
                        case (GameController.CameraMonitor.West_Hall):
                        case (GameController.CameraMonitor.West_Hall_Corner):
                            bonnieLocation = GameController.CameraMonitor.Supply_Closet;
                            break;
                        case (GameController.CameraMonitor.Supply_Closet):
                            bonnieLocation = GameController.CameraMonitor.Outside_Office;
                            break;
                        case (GameController.CameraMonitor.Outside_Office):
                            if (TempData.leftDoorDown) {
                                bonnieLocation = GameController.CameraMonitor.Dining_Area;
                            } else {
                                bonnieLocation = GameController.CameraMonitor.Office;
                            }
                            break;
                        case (GameController.CameraMonitor.Office):
                            //Do nothing, Bonnie is ready to attack
                            break;
                        default:
                            Debug.LogError("Bonnie is in an unintended location");
                            break;
                    }
                }
                //The other half of the locations Bonnie can move to
                else {
                    switch (bonnieLocation) {
                        case (GameController.CameraMonitor.Show_Stage):
                            bonnieLocation = GameController.CameraMonitor.Dining_Area;
                            break;
                        case (GameController.CameraMonitor.Backstage):
                        case (GameController.CameraMonitor.Dining_Area):
                            bonnieLocation = GameController.CameraMonitor.West_Hall;
                            break;
                        case (GameController.CameraMonitor.West_Hall):
                            bonnieLocation = GameController.CameraMonitor.West_Hall_Corner;
                            break;
                        case (GameController.CameraMonitor.West_Hall_Corner):
                            bonnieLocation = GameController.CameraMonitor.Outside_Office;
                            break;
                        case (GameController.CameraMonitor.Supply_Closet):
                            bonnieLocation = GameController.CameraMonitor.West_Hall;
                            break;
                        case (GameController.CameraMonitor.Outside_Office):
                            if (TempData.leftDoorDown) {
                                bonnieLocation = GameController.CameraMonitor.Dining_Area;
                            } else {
                                bonnieLocation = GameController.CameraMonitor.Office;
                            }
                            break;
                        case (GameController.CameraMonitor.Office):
                            //Do nothing, Bonnie is ready to attack
                            break;
                        default:
                            Debug.LogError("Bonnie is in an unintended location");
                            break;
                    }
                }

                if (bonnieOldLoc != bonnieLocation) {
                    //Bonnie can appear near the camera or far away from the camera in some areas
                    bonnieNearCamera = Random.Range(0, 2) == 0;

                    //Play audio signalling Bonnie's movement
                    bonnieSteppingAudio.volume = DetermineFootstepVolume(bonnieLocation);
                    bonnieSteppingAudio.Play();

                    //Update Bonnie's location
                    Messenger<GameController.CameraMonitor>.Broadcast(GameEvent.BONNIE_MOVED, bonnieLocation);
                    Messenger<bool>.Broadcast(GameEvent.BONNIE_NEAR_CAMERA, bonnieNearCamera);
                }
            }
        }

        //Chica movement opportunity
        if (chicaWait >= chicaMoveTimer) {
            chicaWait -= chicaMoveTimer;

            //Chica will move at random based on her AI level
            if (Random.Range(1, maxAI + 1) <= chicaAI && !TempData.playerWon && !TempData.dying) {

                GameController.CameraMonitor chicaOldLoc = chicaLocation;

                //Chica can move to one of two locations based on her current location
                if (Random.Range(0, 2) == 0) {
                    switch (chicaLocation) {
                        case (GameController.CameraMonitor.Show_Stage):
                        case (GameController.CameraMonitor.East_Hall):
                            chicaLocation = GameController.CameraMonitor.Dining_Area;
                            break;
                        case (GameController.CameraMonitor.Dining_Area):
                        case (GameController.CameraMonitor.Kitchen):
                            chicaLocation = GameController.CameraMonitor.Restrooms;
                            break;
                        case (GameController.CameraMonitor.Restrooms):
                            chicaLocation = GameController.CameraMonitor.Kitchen;
                            break;
                        case (GameController.CameraMonitor.East_Hall_Corner):
                            chicaLocation = GameController.CameraMonitor.East_Hall;
                            break;
                        case (GameController.CameraMonitor.Outside_Office):
                            if (TempData.rightDoorDown) {
                                chicaLocation = GameController.CameraMonitor.Dining_Area;
                            } else {
                                chicaLocation = GameController.CameraMonitor.Office;
                            }
                            break;
                        case (GameController.CameraMonitor.Office):
                            //Do nothing, Chica is ready to attack
                            break;
                        default:
                            Debug.LogError("Chica is in an unintended location");
                            break;
                    }
                } else {
                    switch (chicaLocation) {
                        case (GameController.CameraMonitor.Show_Stage):
                            chicaLocation = GameController.CameraMonitor.Dining_Area;
                            break;
                        case (GameController.CameraMonitor.Dining_Area):
                            chicaLocation = GameController.CameraMonitor.Kitchen;
                            break;
                        case (GameController.CameraMonitor.Kitchen):
                        case (GameController.CameraMonitor.Restrooms):
                            chicaLocation = GameController.CameraMonitor.East_Hall;
                            break;
                        case (GameController.CameraMonitor.East_Hall):
                            chicaLocation = GameController.CameraMonitor.East_Hall_Corner;
                            break;
                        case (GameController.CameraMonitor.East_Hall_Corner):
                            chicaLocation = GameController.CameraMonitor.Outside_Office;
                            break;
                        case (GameController.CameraMonitor.Outside_Office):
                            if (TempData.rightDoorDown) {
                                chicaLocation = GameController.CameraMonitor.Dining_Area;
                            } else {
                                chicaLocation = GameController.CameraMonitor.Office;
                            }
                            break;
                        case (GameController.CameraMonitor.Office):
                            //Do nothing, Chica is ready to attack
                            break;
                        default:
                            Debug.LogError("Chica is in an unintended location");
                            break;
                    }
                }

                if (chicaOldLoc != chicaLocation) {
                    //Chica can appear near the camera or far away from the camera in some areas
                    chicaNearCamera = Random.Range(0, 2) == 0;

                    //Play audio signalling Chica's movement
                    chicaSteppingAudio.volume = DetermineFootstepVolume(chicaLocation);
                    chicaSteppingAudio.Play();

                    //Update Chica's location
                    Messenger<GameController.CameraMonitor>.Broadcast(GameEvent.CHICA_MOVED, chicaLocation);
                    Messenger<bool>.Broadcast(GameEvent.CHICA_NEAR_CAMERA, chicaNearCamera);
                }
            }
        }

        //This fairly sizable chunk of code handles the audio playing while Chica is in the kitchen.
        if (chicaLocation == GameController.CameraMonitor.Kitchen) {
            //Once the previous audio clip has finished playing, play a new one
            if (chicaInKitchenSoundCountdown <= 0.0f) {
                for (int i = 0; i < chicaKitchenAudio.Length; i++) {
                    chicaKitchenAudio[i].Stop();
                }
                int kitchenSound = Random.Range(0, chicaKitchenAudio.Length);
                chicaInKitchenSoundCountdown = chicaKitchenAudio[kitchenSound].clip.length;
                chicaKitchenAudio[kitchenSound].Play();
            }
            else { //While an audio clip is playing
                chicaInKitchenSoundCountdown -= Time.deltaTime;
            }

            //Automatically adjusts kitchen clanging volume depending on where the player is looking
            for (int i = 0; i < chicaKitchenAudio.Length; i++) {
                float kitchenVolume;
                switch (TempData.playerViewingCamera)
                {
                    case(GameController.CameraMonitor.Office):
                        kitchenVolume = 0.10f;
                        break;
                    case(GameController.CameraMonitor.Kitchen):
                        kitchenVolume = 0.75f;
                        break;
                    default:
                        kitchenVolume = 0.20f;
                        break;
                }
                if (TempData.playerWon || TempData.dying) {
                    kitchenVolume = 0;
                }
                chicaKitchenAudio[i].volume = kitchenVolume;
            }
        } else { //Stops Chica's kitchen audio once she's left the kitchen
            for (int i = 0; i < chicaKitchenAudio.Length; i++) {
                chicaKitchenAudio[i].Stop();
            }
            chicaInKitchenSoundCountdown = 0.0f;
        }

        /*
        if (bonnieLocation == GameController.CameraMonitor.West_Hall_Corner) {
            bonnieAI = 0;
        }
        if (chicaLocation == GameController.CameraMonitor.East_Hall_Corner) {
            chicaAI = 0;
        }
        //*/

        //Freddy movement opportunity
        if (freddyWait >= freddyMoveTimer) {
            freddyWait -= freddyMoveTimer;

            //Freddy cannot move if Bonnie or Chica are on the show stage, or if the player is looking at him
            if (!(bonnieLocation == GameController.CameraMonitor.Show_Stage || chicaLocation == GameController.CameraMonitor.Show_Stage || TempData.playerViewingCamera == freddyLocation)) {
                //Freddy will move at random based on his AI level
                if (Random.Range(1, maxAI + 1) <= freddyAI) {
                    if (freddyDelay == 0.0f && GetFreddyDelayTimer() > 0.0f) {
                        StartCoroutine("FreddyDelayCount");
                    }
                }
                if (freddyDelay >= GetFreddyDelayTimer()) {
                    freddyDelay = 0.0f;
                    StopCoroutine("FreddyDelayCount");
                    GameController.CameraMonitor freddyOldLoc = freddyLocation;
                    if (TempData.playerViewingCamera != freddyLocation && !TempData.playerWon && !TempData.dying) {
                        switch (freddyLocation) {
                            case (GameController.CameraMonitor.Show_Stage):
                                freddyLocation = GameController.CameraMonitor.Dining_Area;
                                break;
                            case (GameController.CameraMonitor.Dining_Area):
                                freddyLocation = GameController.CameraMonitor.Restrooms;
                                break;
                            case (GameController.CameraMonitor.Restrooms):
                                freddyLocation = GameController.CameraMonitor.Kitchen;
                                freddyMusic.Play();
                                break;
                            case (GameController.CameraMonitor.Kitchen):
                                freddyLocation = GameController.CameraMonitor.East_Hall;
                                break;
                            case (GameController.CameraMonitor.East_Hall):
                                freddyLocation = GameController.CameraMonitor.East_Hall_Corner;
                                break;
                            case (GameController.CameraMonitor.East_Hall_Corner):
                                if (TempData.playerViewingCamera == GameController.CameraMonitor.Office) { //Freddy cannot move into the office if the player is not looking at the security cameras
                                    freddyLocation = GameController.CameraMonitor.East_Hall_Corner;
                                } else if (TempData.rightDoorDown) {
                                    freddyLocation = GameController.CameraMonitor.East_Hall;
                                } else {
                                    freddyLocation = GameController.CameraMonitor.Office;
                                    freddyOfficeAudio.Play();
                                }
                                break;
                            case (GameController.CameraMonitor.Office):
                                //Do nothing, Freddy is ready to attack
                                break;
                            default:
                                Debug.LogError("Freddy is in an unintended location");
                                break;
                        }
                        if (freddyOldLoc != freddyLocation) {
                            PlayFreddyLaugh(freddyOldLoc, freddyLocation);
                            Messenger<GameController.CameraMonitor>.Broadcast(GameEvent.FREDDY_MOVED, freddyLocation);
                        }
                    }
                }
            }
        }
        //Automatically adjusts Freddy's music box volume while he's in the kitchen
        if (freddyLocation == GameController.CameraMonitor.Kitchen) {
            //Automatically adjusts volume depending on where the player is looking
            float kitchenVolume;
            switch (TempData.playerViewingCamera)
            {
                case(GameController.CameraMonitor.Office):
                    kitchenVolume = 0.10f;
                    break;
                case(GameController.CameraMonitor.Kitchen):
                    kitchenVolume = 0.50f;
                    break;
                default:
                    kitchenVolume = 0.15f;
                    break;
            }
            freddyMusic.volume = kitchenVolume;
        } else { //Stops Freddy's kitchen audio once he's left the kitchen
            freddyMusic.Stop();
        }

        //Foxy movement opportunity
        if (foxyWait >= foxyMoveTimer) {
            foxyWait -= foxyMoveTimer;

            //Foxy cannot move if the player is looking at the cameras or if his penalty timer is active
            if (TempData.playerViewingCamera == GameController.CameraMonitor.Office && foxyPenalty <= 0) {
                if (Random.Range(1, maxAI + 1) <= foxyAI && !TempData.playerWon && !TempData.dying) {
                    //Foxy can be in one of 4 positions (position 0, 1, 2, or 3)
                    // 0 is behind the curtain
                    // 1 is peaking
                    // 2 is leaning out
                    // 3 means he left, and the player is in danger
                    TempData.foxyPrep += 1;
                    Messenger.Broadcast(GameEvent.FOXY_MOVED);
                }
            }

            //Foxy kill behaviour and progress resetting is handled by GameController.cs
        }
        foxyPenalty -= Time.deltaTime;

        //Golden Freddy behavior
        //Golden Freddy has a 1/10,000 chance every second to be set to appear in the West Hall Corner.
        //If he is set to appear, the poster in the West Hall Corner will display his face.
        //If the player views the West Hall Corner while Bonnie is not there and the cameras are not disabled, Golden Freddy will be set to appear in the office.
        //If the player lowers the monitor and does not lift it back up for the duration of 5 seconds, Golden Freddy will kill the player (by loading his kill screen scene)
        //If the player manages to raise the monitor after seeing Golden Freddy, he will not appear again for the duration of the night.

        //If Golden Freddy has not been set to appear
        if (goldenFreddyStatus == GoldenFreddyStates.Waiting 
        && goldenFreddyWait >= goldenFreddyMoveTimer) {

            goldenFreddyWait -= goldenFreddyMoveTimer;

            //Golden Freddy has a 1/10,000 chance every second to be set to appear in the West Hall Corner.
            //This automatically fails if the player is already viewing the West Hall Corner
            if (Random.Range(0, goldenFreddyChance) == 0
            && TempData.playerViewingCamera != GameController.CameraMonitor.West_Hall_Corner) {
                SetGoldenFreddyStatus(GoldenFreddyStates.Poster);
            }

        //If Golden Freddy has been set to appear in the West Hall Corner and the player views the West Hall Corner while Bonnie isn't there
        } else if (goldenFreddyStatus == GoldenFreddyStates.Poster 
        && TempData.playerViewingCamera == GameController.CameraMonitor.West_Hall_Corner
        && !TempData.camerasDisabled
        && bonnieLocation != GameController.CameraMonitor.West_Hall_Corner) {

            //Set Golden Freddy to appear in the office
            SetGoldenFreddyStatus(GoldenFreddyStates.Office);

        //If Golden Freddy has been set to appear in the office
        } else if (goldenFreddyStatus == GoldenFreddyStates.Office) { //UpdateTimes() handles the requirement of the player looking in the office
            if (goldenFreddyWait >= goldenFreddyKillTimer && !TempData.dying && !TempData.playerWon) {
                SceneManager.LoadScene("Golden Freddy");
            }
            //Player protects themselves by raising the monitor, which sets Golden Freddy's status to Finished.
            //Was gonna put an else/if here but ended up just using GameEvent.SWITCH_TO_MONITOR instead
        }
    }

    void UpdateTimes() {
        bonnieWait += Time.deltaTime;
        chicaWait += Time.deltaTime;
        freddyWait += Time.deltaTime;
        foxyWait += Time.deltaTime;

        //If Golden Freddy hasn't been set to appear 
        if (goldenFreddyStatus == GoldenFreddyStates.Waiting) {
            goldenFreddyWait += Time.deltaTime;
        }
        //if both Golden Freddy and the player are in the office
        else if (goldenFreddyStatus == GoldenFreddyStates.Office && TempData.playerViewingCamera == GameController.CameraMonitor.Office) {
            goldenFreddyWait += Time.deltaTime;
            TempData.manuallyHallucinate = true;
        } else {
            goldenFreddyWait = 0.0f;
            TempData.manuallyHallucinate = false;
        }

        if (TempData.playerViewingCamera != GameController.CameraMonitor.Office) {
            foxyPenalty = Random.Range(0.833f, 17.5f); //50-1050 ticks
        }
    }

    private float DetermineFootstepVolume(GameController.CameraMonitor monitor) {
        switch (monitor) {
            case(GameController.CameraMonitor.Dining_Area):
            case(GameController.CameraMonitor.Backstage):
            case(GameController.CameraMonitor.Restrooms):
                return 0.10f;
            case(GameController.CameraMonitor.Supply_Closet):
            case(GameController.CameraMonitor.Kitchen):
                return 0.15f;
            case(GameController.CameraMonitor.West_Hall):
            case(GameController.CameraMonitor.East_Hall):
                return 0.20f;
            case(GameController.CameraMonitor.West_Hall_Corner):
            case(GameController.CameraMonitor.East_Hall_Corner):
                return 0.25f;
            case(GameController.CameraMonitor.Outside_Office):
                return 0.30f;
            default:
                return 0.00f;
        }
    }

    //Set to default values
    void Reset() 
    {
        bonnieWait = 0.0f;
        chicaWait = 0.0f;
        freddyWait = 0.0f;
        foxyWait = 0.0f;
        goldenFreddyWait = 0.0f;

        bonnieLocation = GameController.CameraMonitor.Show_Stage;
        chicaLocation = GameController.CameraMonitor.Show_Stage;
        freddyLocation = GameController.CameraMonitor.Show_Stage;
        TempData.foxyPrep = 0;
        goldenFreddyStatus = GoldenFreddyStates.Waiting;    //TODO: This should call SetGoldenFreddyStatus but because Reset() is called before the first frame, GoldenFreddyWHallCorner.cs gets a NullReferenceException when doing that.
                                                            //This would likely be fixed by adding a frame delay via a coroutine when setting Golden Freddy's initial status.
                                                            //This also won't cause a bug unless you want to reset the night in the middle of the night which like, no.

        TempData.leftDoorDown = false;
        TempData.rightDoorDown = false;
    }

    void StartNight()
    {
        //Set difficulty
        if (TempData.loadNight == 0) { //loadNight will be 0 if the "FNaF Night" scene is launched directly. This is used for testing.
            bonnieAI = 20;
            chicaAI = 20;
            freddyAI = 20;
            foxyAI = 20;
        }
        else {
            bonnieAI = TempData.bonnieAI;
            chicaAI = TempData.chicaAI;
            freddyAI = TempData.freddyAI;
            foxyAI = TempData.foxyAI;
        }

        Debug.Log("Starting night " + TempData.loadNight + " with difficulty [" + freddyAI + ", " + bonnieAI + ", " + chicaAI + ", " + foxyAI + "]");
    }

    //Called when the hour is updated. Gives a slight difficulty boost to Bonnie, Chica, and Foxy as the night progresses. (2AM, 3AM, and 4AM)
    void OnTimeChanged(int time)
    {
        switch (time)
        {
            case(2):
                bonnieAI++;
                break;
            case(3):
            case(4):
                bonnieAI++;
                chicaAI++;
                foxyAI++;
                break;
            default:
                break;
        }
    }

    private void OnSwitchToMonitor() 
    {
        if (goldenFreddyStatus == GoldenFreddyStates.Office) {
            SetGoldenFreddyStatus(GoldenFreddyStates.Finished);
        }
    }

    public void SetGoldenFreddyStatus(GoldenFreddyStates status)
    {
        goldenFreddyStatus = status;
        Messenger<GoldenFreddyStates>.Broadcast(GameEvent.GOLDEN_FREDDY_UPDATE, goldenFreddyStatus);

        if (status == GoldenFreddyStates.Office) {
            goldenFreddyGiggleAudio.Play();
        }
    } 

    //How long Freddy is delayed before he can move, in seconds.
    private float GetFreddyDelayTimer()
    {
        //1000 ticks at base, - 100 ticks for every AI level above 0.
        float delay = (1000.0f - (freddyAI * 100.0f)) / 60.0f;
        return delay;
    }

    private void PlayFreddyLaugh(GameController.CameraMonitor from, GameController.CameraMonitor to)
    {
        float freddyLaughVolume = 0.0f;
        float freddyStepVolume = 0.0f;
        
        if (from == GameController.CameraMonitor.Show_Stage) {
            freddyLaughVolume = 0.15f;
            freddyStepVolume = 0.30f;
        } else if (from == GameController.CameraMonitor.Dining_Area) {
            freddyLaughVolume = 0.20f;
            freddyStepVolume = 0.35f;
        } else if (from == GameController.CameraMonitor.Restrooms) {
            freddyLaughVolume = 0.30f;
            freddyStepVolume = 0.40f;
        } else if (from == GameController.CameraMonitor.Kitchen) {
            freddyLaughVolume = 0.40f;
            freddyStepVolume = 0.60f;
        } else if (from == GameController.CameraMonitor.East_Hall) {
            freddyLaughVolume = 0.60f;
            freddyStepVolume = 0.75f;
        } else if (from == GameController.CameraMonitor.East_Hall_Corner) {
            if (to == GameController.CameraMonitor.East_Hall) {
                freddyLaughVolume = 0.60f;
                freddyStepVolume = 0.75f;
            } else if (to == GameController.CameraMonitor.Office) {
                freddyLaughVolume = 0.80f;
                freddyStepVolume = 1.0f;
            } else {
                Debug.Log("Freddy will not laugh");
            }
        } else {
            Debug.Log("Freddy will not laugh");
        }

        int whichLaugh = Random.Range(0, freddyLaughAudio.Length);
        freddyLaughAudio[whichLaugh].volume = freddyLaughVolume;
        freddySteppingAudio.volume = freddyStepVolume;
        freddyLaughAudio[whichLaugh].Play();
        freddySteppingAudio.Play();
    }

    private IEnumerator FreddyDelayCount()
    {
        while (true) {
            freddyDelay += Time.deltaTime;
            yield return null;
        }
    }
}
