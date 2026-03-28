using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles the vast majority of the Night behavior.
public class GameController : MonoBehaviour
{
    public enum CameraMonitor
    {
        Office,
        Show_Stage,
        Dining_Area,
        Pirate_Cove,
        West_Hall,
        West_Hall_Corner,
        Supply_Closet,
        East_Hall,
        East_Hall_Corner,
        Backstage,
        Kitchen,
        Restrooms,
        Outside_Office,
        Win_Screen
    }

    //Delay for switching between screens following a death
    private static float timeOnStatic = 3.0f;
    private static float timeFading = 1.0f;
    [SerializeField] private Renderer gameOverStaticRenderer; //The static that appears immediately upon a game over
    private static int gameOverGoldenFreddyChance = 10000; // 1/X chance that the game over screen loads Golden Freddy's kill screen instead of the main menu.
    private static float timeOnEndScreen = 5.25f; //Time spent on the game over screen, in addition to the time spent staring at static
    private static float timeMovingNumbers = 170.0f / 30.0f; //Ticks measured in 30th's of a second
    [SerializeField] private GameObject WinScreenNumber; //The number "5" on the win screen, which scrolls up to the cheer of children
    [SerializeField] private GameObject[] FadeOutOnWin; //Array of objects to fade out upon winning. As of writing, contains the number "6" as well as the "AM" text

    [SerializeField] private GameObject[] disableOnGameOver; //Various objects around the office to disable upon a game over

    //Various sounds
    [SerializeField] private AudioSource blipClip; //Camera blip upon changing cameras
    [SerializeField] private AudioSource staticAudio; //Static sound effect played upon a game over
    [SerializeField] private AudioSource errorAudio; //Office button error sound
    [SerializeField] private AudioSource honkAudio; //Freddy poster honk sound
    [SerializeField] private AudioSource windowStingerAudio; //Stinger played when turning the lights on while Bonnie/Chica are there
    [SerializeField] private AudioSource winScreenClockChimeAudio; //Grandfather clock chime
    [SerializeField] private AudioSource winScreenCheerAudio; //Children cheering
    [SerializeField] private AudioSource foxySprintAudio; //Foxy racing down the West Hall
    [SerializeField] private AudioSource foxyKnockAudio; //Foxy failing to enter the office
    [SerializeField] private AudioSource camerasDisabledAudio; //Cameras disabled because an animatronic is moving to/from the currently viewed camera
    [SerializeField] private AudioSource[] raspyBreathAudio; //Array of audio clips that randomly play when the camera is up while Bonnie/Chica are in the office

    [SerializeField] private Camera[] cameras; //Array of cameras used to change between monitors
    private CameraMonitor currentMonitor = CameraMonitor.Show_Stage; //Saved monitor, which is switched to upon raising the monitor

    [SerializeField] private Animator backgroundAnim; //Office texture animator

    //Animators and game objects (hit boxes) for the buttons on the left and right sides of the main office
    [SerializeField] private Animator leftSideButtonsAnim;
    [SerializeField] private Animator leftSideDoorAnim;
    [SerializeField] private GameObject leftSideDoorButton;
    [SerializeField] private GameObject leftSideLightButton;

    [SerializeField] private Animator rightSideButtonsAnim;
    [SerializeField] private Animator rightSideDoorAnim;
    [SerializeField] private GameObject rightSideLightButton;
    [SerializeField] private GameObject rightSideDoorButton;
    
    [SerializeField] private GameObject honkNosePoster; //Hit box for the poster honk

    //Current locations for chica/bonnie/freddy
    private CameraMonitor chicaLoc = CameraMonitor.Show_Stage;
    private CameraMonitor bonnieLoc = CameraMonitor.Show_Stage;
    private Animatronics.Names whichAnimatronicEnteredFirst = Animatronics.Names.None; //Used to determine whether Bonnie or Chica will jumpscare the player when one of them enters the office
    private bool chicaWindowScare = false; //Allows the stinger to be played when Chica/Bonnie are outside the office
    private bool bonnieWindowScare = false;
    private CameraMonitor freddyLoc = CameraMonitor.Show_Stage;

    // 0 = behind curtains
    // 1 = peaking
    // 2 = leaning
    // 3 = gone
    // when TempData.foxyPrep = 3, the player will be attacked in foxyAttackTimer seconds. If the player looks at the west hall during this time they will be attacked in foxyHallAttackTimer seconds instead.
    private static int foxyPrepDanger = 3; //Constant that refers to when the player is in danger of being attacked by foxy
    private bool foxySeenInWestHall = false; //Used when Foxy is seen in the left hall so the player can't just rapidly switch back and forth from the West Hall to reset the kill timer.

    private static float foxyAttackTimer = 25.0f; //1500 ticks which I believe is 1/60th of a second each
    private static float foxyHallAttackTimer = 100.0f / 60.0f; //About 1.666etc seconds. 100 ticks which I believe are 1/60th of a second each
    private float playerInDangerFoxy = 0.0f; //Timer before Foxy attacks, which checks against foxyAttackTimer and indirectly against foxyHallAttackTimer
    private int foxyAttackedTimes = 0; //Number of times Foxy has attacked in a single night
    private static float bonnieChicaAttackTimer = 30.0f; //How long it takes Bonnie/Chica to get impatient and kill the player while they're staring at the monitor.
    private float playerInDangerMonitor = 0.0f; //Timer before Bonnie or Chica attacks when the player is looking at the monitor for too long
    private float raspyBreathWait = 0.0f; //Keeps track of how long it's been since the last raspy breath
    private float raspyBreathTimer = 5.0f; //How often to play a raspy breath sound while Bonnie/Chica are in the office while the player is looking at the monitor
    private static float freddyAttackTimer = 1.0f; //Freddy has one chance to attack every second that the player is not looking at the monitor.
    private float playerInDangerFreddy = 0.0f; //Timer before Freddy attacks

    //Office lights
    private bool leftLightOn = false;
    private bool rightLightOn = false;

    //Clock timers.
    private float minuteHand = 0.0f; //Counts up to minutesPerHour before adding an hour to the clock
    private int hourHand = 0;
    private static float minutesPerHour = 90.0f;

    [SerializeField] private SpriteRenderer winScreenScreenShot; //Game takes a screenshot and sends it to the win screen, which is then made to fade out

    [SerializeField] private GameObject hallucinationOverlay;

    //Detects where the mouse is
    private Ray ray;
    private RaycastHit hit;

    void Awake()
    {
        Messenger.AddListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
        Messenger.AddListener(GameEvent.SWITCH_TO_MONITOR, OnSwitchToMonitor);
        Messenger<CameraMonitor>.AddListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger<CameraMonitor>.AddListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<CameraMonitor>.AddListener(GameEvent.CHICA_MOVED, OnChicaMoved);
        Messenger<CameraMonitor>.AddListener(GameEvent.FREDDY_MOVED, OnFreddyMoved);
        Messenger.AddListener(GameEvent.JUMPSCARE, OnJumpscare);
        Messenger.AddListener(GameEvent.GAME_OVER, OnGameOver);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
        Messenger.RemoveListener(GameEvent.SWITCH_TO_MONITOR, OnSwitchToMonitor);
        Messenger<CameraMonitor>.RemoveListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger<CameraMonitor>.RemoveListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<CameraMonitor>.RemoveListener(GameEvent.CHICA_MOVED, OnChicaMoved);
        Messenger<CameraMonitor>.RemoveListener(GameEvent.FREDDY_MOVED, OnFreddyMoved);
        Messenger.RemoveListener(GameEvent.JUMPSCARE, OnJumpscare);
        Messenger.RemoveListener(GameEvent.GAME_OVER, OnGameOver);
    }

    // Start is called before the first frame update
    void Start()
    {
        TempData.playerViewingCamera = CameraMonitor.Office;
        TempData.leftDoorDown = false;
        TempData.rightDoorDown = false;
        TempData.dying = false;
        TempData.playerWon = false;
        //Set all cameras besides the main camera to inactive
        SwitchToCamera(CameraMonitor.Office);
        hallucinationOverlay.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Where the mouse is pointing
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the mouse is pointing at something
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //click the "leftSideLightButton" GameObject
            if (hit.collider.name == leftSideLightButton.name) {
                if (bonnieLoc == CameraMonitor.Office) {
                    errorAudio.Play();
                } else {
                    rightLightOn = false;
                    leftLightOn = !leftLightOn;

                    if (bonnieWindowScare && leftLightOn) {
                        bonnieWindowScare = false;
                        windowStingerAudio.Play();
                    }
                }
            }

            //click the "rightSideLightButton" GameObject
            if (hit.collider.name == rightSideLightButton.name) {
                if (chicaLoc == CameraMonitor.Office) {
                    errorAudio.Play();
                } else {
                    rightLightOn = !rightLightOn;
                    leftLightOn = false;

                    if (chicaWindowScare && rightLightOn) {
                        chicaWindowScare = false;
                        windowStingerAudio.Play();
                    }
                }
            }

            //click the "leftSideDoorButton" GameObject
            if (hit.collider.name == leftSideDoorButton.name) {
                if (bonnieLoc == CameraMonitor.Office) {
                    errorAudio.Play();
                } else {
                    TempData.leftDoorDown = !TempData.leftDoorDown;
                    //Door animation happens at the end of Update()
                }
            }

            //click the "rightSideDoorButton" GameObject
            if (hit.collider.name == rightSideDoorButton.name) {
                if (chicaLoc == CameraMonitor.Office) {
                    errorAudio.Play();
                } else {
                    TempData.rightDoorDown = !TempData.rightDoorDown;
                    //Door animation happens at the end of Update()
                }
            }

            //click the "honkNosePoster" GameObject
            if (hit.collider.name == honkNosePoster.name) {
                honkAudio.Play();
            }
        }

        TempData.lightOn = leftLightOn || rightLightOn; //Used to calculate power drainage

        //Freddy/Chica/Bonnie danger calculators
        if (TempData.playerViewingCamera != CameraMonitor.Office) { //If the player is looking at the cameras
            if (bonnieLoc == CameraMonitor.Office || chicaLoc == CameraMonitor.Office) { //If Bonnie or Chica are in the office
                playerInDangerMonitor += Time.deltaTime; //Bonnie or Chica will kill the player if they are looking at the cameras for 30 seconds while they're in the office
            }
            playerInDangerFreddy = 0.0f; //Resets Freddy's kill timer when the player is looking at the cameras
        } else { //If the player is not looking at the cameras
            if (freddyLoc == CameraMonitor.Office) { //If Freddy is in the office
                playerInDangerFreddy += Time.deltaTime; //Freddy has a 1/4 chance to kill the player every second if the player is not looking at the cameras while he's in the office
            }
        }
        //If player has been looking at the monitor for 30 seconds while in danger
        if (playerInDangerMonitor >= bonnieChicaAttackTimer && !TempData.dying && !TempData.playerWon) {
            PlayBonnieChicaJumpscare();
        //Every second that Freddy is in the office and the player doesn't have the monitor up, there's a 1/4 chance that Freddy kills the player
        } else if (playerInDangerFreddy >= freddyAttackTimer && !TempData.dying && !TempData.playerWon) {
            playerInDangerFreddy -= freddyAttackTimer;
            if (Random.Range(0, 4) == 0 && freddyLoc == CameraMonitor.Office && TempData.playerViewingCamera == CameraMonitor.Office) {
                backgroundAnim.SetBool("FreddyJumpscare", true);
            }
        }

        //Foxy behaviour
        //If Foxy has left Pirate's Cove
        if (TempData.foxyPrep >= foxyPrepDanger && !TempData.dying && !TempData.playerWon) {
            playerInDangerFoxy += Time.deltaTime; //Increment kill timer
            //If player looks at the West Hall while the cameras aren't disabled from an animatronic moving
            if (!foxySeenInWestHall && TempData.playerViewingCamera == CameraMonitor.West_Hall && !TempData.camerasDisabled) {
                foxySeenInWestHall = true; //Prevents the player from resetting Foxy's kill timer by switching to and from the West Hall repeatedly.
                playerInDangerFoxy = foxyAttackTimer - foxyHallAttackTimer; //Foxy will attack in foxyHallAttackTimer seconds.
                foxySprintAudio.Play();
            }
        }
        else { //Keep Foxy's kill variables at 0 if he hasn't left Pirate's Cove / the game is over
            playerInDangerFoxy = 0.0f;
            foxySeenInWestHall = false;
        }

        //Foxy attack timer is up
        if (playerInDangerFoxy >= foxyAttackTimer && !TempData.dying && !TempData.playerWon) {
            if (!TempData.leftDoorDown) {
                backgroundAnim.SetBool("FoxyJumpscare", true);
                Messenger.Broadcast(GameEvent.FORCE_CAMERA_LEFT);
            } else {
                TempData.foxyPrep = 0; //Reset Foxy
                Messenger.Broadcast(GameEvent.FOXY_MOVED);
                Debug.Log("Foxy failed to kill the player.");
                foxyKnockAudio.Play();
                TempData.remainingPower -= 10 + (50 * foxyAttackedTimes);
                foxyAttackedTimes++;
            }
            foxySprintAudio.Stop();
        }


        //Assure that when player wins, the player actually *wins*
        if (TempData.playerWon) {
            TempData.dying = false;
        } 

        if (TempData.playerWon || TempData.dying) {
            playerInDangerMonitor = 0.0f;
            playerInDangerFreddy = 0.0f;
            playerInDangerFoxy = 0.0f;
            leftLightOn = false;
            rightLightOn = false;
        } else if (playerInDangerMonitor > 0.0f) {
            raspyBreathWait += Time.deltaTime;

            if (raspyBreathWait >= raspyBreathTimer) {
                raspyBreathWait -= raspyBreathTimer;
                int whichRaspyBreath = Random.Range(0, raspyBreathAudio.Length);
                raspyBreathAudio[whichRaspyBreath].Play();
            }
        }

        //An hour in-game lasts 90 seconds
        minuteHand += Time.deltaTime;
        if (minuteHand >= minutesPerHour && !TempData.playerWon) {
            minuteHand -= minutesPerHour;
            hourHand++;
            Messenger<int>.Broadcast(GameEvent.TIME_CHANGE, hourHand);
            //Debug.Log("It is now " + hourHand + ":00AM");
        }

        //A night lasts 6 hours
        if (hourHand >= 6 && !TempData.playerWon) {
            DoGameWon();
        }

        leftSideDoorAnim.SetBool("LDoorClosed", TempData.leftDoorDown);
        leftSideButtonsAnim.SetBool("DoorButtonPressed", TempData.leftDoorDown);
        rightSideDoorAnim.SetBool("RDoorClosed", TempData.rightDoorDown);
        rightSideButtonsAnim.SetBool("DoorButtonPressed", TempData.rightDoorDown);
        backgroundAnim.SetBool("LeftLightOn", leftLightOn);
        backgroundAnim.SetBool("RightLightOn", rightLightOn);
        leftSideButtonsAnim.SetBool("LightButtonPressed", leftLightOn);
        rightSideButtonsAnim.SetBool("LightButtonPressed", rightLightOn);
    }

    //Switches the camera to the specified CameraMonitor
    //For example, SwitchToCamera(CameraMonitor.Office) would swap to the main office
    //Exception being "CameraMonitor.Outside_Office" switches to the lose screen
    private void SwitchToCamera(CameraMonitor cameraMonitor)
    {
        int cameraIndex = (int)cameraMonitor;
        TempData.playerViewingCamera = cameraMonitor;

        for (int i = 0; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(false);
        }

        cameras[cameraIndex].gameObject.SetActive(true);
    }

    private void OnSwitchToMonitor()
    {
        if (!TempData.dying && !TempData.playerWon) {
            SwitchToCamera(currentMonitor);
            leftLightOn = false;
            rightLightOn = false;
            blipClip.Play();
        }
    }

    private void OnSwitchToOffice()
    {
        if (!TempData.playerWon) {
            SwitchToCamera(CameraMonitor.Office);
            if (playerInDangerMonitor > 0.0f) {
                PlayBonnieChicaJumpscare();
            }
        }
    }

    private void PlayBonnieChicaJumpscare()
    {
        if (whichAnimatronicEnteredFirst == Animatronics.Names.Bonnie) { //If Bonnie entered first
            backgroundAnim.SetBool("BonnieJumpscare", true); 
        } else if (whichAnimatronicEnteredFirst == Animatronics.Names.Chica) { //If Chica entered first
            backgroundAnim.SetBool("ChicaJumpscare", true);
        } else { //If this function was called despite neither Bonnie or Chica being in the office 
            Debug.LogError("Player is in danger but neither Bonnie or Chica are in the office");
        }
    }

    private void OnChangeCamera(CameraMonitor monitor)
    {
        if (!TempData.dying && !TempData.playerWon) {
            currentMonitor = monitor;
            SwitchToCamera(monitor);
            blipClip.Play();
        }
    }

    private void OnBonnieMoved(CameraMonitor monitor)
    {
        if (bonnieLoc != monitor) {
            DoPlayerSawAnimatronicMove(bonnieLoc, monitor);
            bonnieLoc = monitor;

            if (bonnieLoc == CameraMonitor.Office) {
                Debug.Log("Bonnie is in the office");
                leftLightOn = false;
                if (whichAnimatronicEnteredFirst == Animatronics.Names.None) {
                    whichAnimatronicEnteredFirst = Animatronics.Names.Bonnie;
                }
            } 
            
            bonnieWindowScare = bonnieLoc == CameraMonitor.Outside_Office;
        }
    }

    private void OnChicaMoved(CameraMonitor monitor)
    {
        if (chicaLoc != monitor) {
            DoPlayerSawAnimatronicMove(chicaLoc, monitor);
            chicaLoc = monitor;

            if (chicaLoc == CameraMonitor.Office) {
                Debug.Log("Chica is in the office");
                rightLightOn = false;
                if (whichAnimatronicEnteredFirst == Animatronics.Names.None) {
                    whichAnimatronicEnteredFirst = Animatronics.Names.Chica;
                }
            }
            
            chicaWindowScare = chicaLoc == CameraMonitor.Outside_Office;
        }
    }

    private void OnFreddyMoved(CameraMonitor monitor)
    {
        DoPlayerSawAnimatronicMove(freddyLoc, monitor); //Freddy shouldn't be able to move if the player is looking at the camera he's on so this will only check if the player is on the camera he's moving to
        freddyLoc = monitor;

        //*
        if (freddyLoc == CameraMonitor.Office) {
            Debug.Log("Freddy is in the office");
        }
        //*/
    }

    private void DoPlayerSawAnimatronicMove(CameraMonitor from, CameraMonitor to)
    {
        //Animatronic actually moved
        if (from != to) {
            //Player is looking at the camera the animatronic is on or moving to, not including the Kitchen or the office itself
            if ((TempData.playerViewingCamera == from || TempData.playerViewingCamera == to) && TempData.playerViewingCamera != CameraMonitor.Office && TempData.playerViewingCamera != CameraMonitor.Kitchen) {
                camerasDisabledAudio.Play();
                Messenger.Broadcast(GameEvent.DISABLE_CAMERAS);
            }
        }
    }

    private void DoGameWon() {
        StartCoroutine("GameWonWithDelay");
    }

    private void OnJumpscare()
    {
        TempData.dying = true;
        for (int i = 0; i < raspyBreathAudio.Length; i++) {
            raspyBreathAudio[i].Stop();
        }
        for (int i = 0; i < disableOnGameOver.Length; i++) {
            disableOnGameOver[i].SetActive(false);
        }
    }

    private void OnGameOver() 
    {
        minuteHand = 0.0f; //Set the minute to :00 so the player can't win after they're fully dead.
        backgroundAnim.SetBool("BonnieJumpscare", false);
        backgroundAnim.SetBool("ChicaJumpscare", false);
        backgroundAnim.SetBool("FreddyJumpscare", false);
        backgroundAnim.SetBool("FoxyJumpscare", false);
        backgroundAnim.SetBool("PowerOutJumpscare", false);
        TempData.playerWon = false;
        StartCoroutine("GameOverWithDelay");
    }

    IEnumerator GameWonWithDelay() {
        StopAllAudio();

        TempData.playerWon = true;

        //Take a screenshot to apply to the win screen
        yield return StartCoroutine("TakeScreenShot");

        //Switch to the actual win screen and fade the screenshot out
        SwitchToCamera(CameraMonitor.Win_Screen);
        winScreenClockChimeAudio.Play();
        for (float i = timeFading; i > 0; i -= Time.deltaTime) {
            // set color with i as alpha
            winScreenScreenShot.material.color = new Color(1, 1, 1, i / timeFading);
            yield return null;
        }
        winScreenScreenShot.material.color = new Color(1, 1, 1, 0);

        //Move the number up
        float elapsedTime = 0;
        Vector3 startingPos = WinScreenNumber.transform.position;
        while (elapsedTime < timeMovingNumbers) {
            WinScreenNumber.transform.position = Vector3.Lerp(startingPos, startingPos + new Vector3(0, 0, -1.4f), (elapsedTime / timeMovingNumbers));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        WinScreenNumber.transform.position = startingPos + new Vector3(0, 0, -1.4f);
        winScreenCheerAudio.Play();

        //Wait timeOnEndScreen seconds
        for (float i = timeOnEndScreen; i > 0; i -= Time.deltaTime) {
            yield return null;
        }

        //Fade out all text over the span of timeFading seconds
        for (float i = timeFading; i > 0; i -= Time.deltaTime) {
            for (int j = 0; j < FadeOutOnWin.Length; j++) {
                FadeOutOnWin[j].GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, i / timeFading);
            }
            yield return null;
        }
        for (int i = 0; i < FadeOutOnWin.Length; i++) {
            FadeOutOnWin[i].GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0);
        }

        //Switch back to the main menu, which will automatically start the next night if applicable
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator GameOverWithDelay()
    {
        StopAllAudio();
        
        //Stay on static screen for timeOnStatic seconds
        //Literal static like on a TV
        staticAudio.Play();
        SwitchToCamera(CameraMonitor.Outside_Office); //Using the "Outside_Office" camera that's used for Bonnie and Chica being outside the office as the static camera
        for (float i = timeOnStatic; i > 0; i -= Time.deltaTime) {
            yield return null;
        }

        //Fade the static away over timeFading seconds
        for (float i = timeFading; i > 0; i -= Time.deltaTime) {
            // set color with i as alpha
            gameOverStaticRenderer.material.color = new Color(1, 1, 1, i / timeFading);
            staticAudio.volume = i / timeFading;
            yield return null;
        }
        gameOverStaticRenderer.material.color = new Color(1, 1, 1, 0);
        staticAudio.Stop();

        //Stay on Game Over screen for timeOnEndScreen seconds
        for (float i = timeOnEndScreen; i > 0; i -= Time.deltaTime) {
            yield return null;
        }

        TempData.playerWon = false; //Main menu handles the win/lose state

        //There's a small chance that instead of loading the main menu, the game loads Golden Freddy's kill screen instead.
        if (Random.Range(0, gameOverGoldenFreddyChance) == 0) {
            SceneManager.LoadScene("Golden Freddy");
        } else {
            SceneManager.LoadScene("Main Menu");
        }
    }

    private IEnumerator TakeScreenShot()
    {
        Camera _camera = cameras[(int)TempData.playerViewingCamera];
        Texture2D _screenShot;
        int resWidth = 1600;
        int resHeight = 720;

        //yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        _camera.targetTexture = rt;
        _screenShot= new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        _camera.Render();
        RenderTexture.active = rt;
        _screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        _screenShot.Apply();
        _camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        //100.8f is the pixels per unit size. Default is 100. Had to bump it up a smidge because the default had the screenshot zooming in. From what I'm eyeballing I can't see any zoom with this new value.
        Sprite tempSprite = Sprite.Create(_screenShot, new Rect(0,0,resWidth,resHeight), new Vector2(0.5f, 0.5f), 100.8f);
        winScreenScreenShot.sprite = tempSprite;
        yield return new WaitForEndOfFrame();
    }

    private void StopAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach( AudioSource audioS in allAudioSources) {
            audioS.Stop();
        }
    }
}
