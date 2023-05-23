using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private enum Item
    {
        New_Game,
        Continue,
        Night_Six,
        Custom_Night
    }

    private enum MenuCamera
    {
        Main_Menu,
        Night_Fade,
        Custom_Night
    }

    public enum StoryBeat
    {
        None,
        New_Game,
        Beat_Game,
        Beat_6,
        Beat_420
    }

    private int[,] AILevels = new int[,] { 
                                {0, 0, 0, 0}, 
                                {0, 3, 1, 1}, 
                                {1, 0, 5, 2}, 
                                {1, 2, 4, 6}, 
                                {3, 5, 7, 5}, 
                                {4, 10, 12, 6}
                            };

    private Item currentItem = Item.New_Game;
    private Item latestItem;
    private Animator anim;

    [SerializeField] private Camera[] cameras;

    [SerializeField] private GameObject hiderPlane; 

    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource staticAudio;
    [SerializeField] private AudioSource blipClip;

    [SerializeField] private GameObject menuItemsContainer;
    [SerializeField] private GameObject newGameButtonLoc;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueButtonLoc;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject continueNightText;
    [SerializeField] private Animator continueNightNumber;
    [SerializeField] private GameObject nightSixButtonLoc;
    [SerializeField] private GameObject nightSixButton;
    [SerializeField] private GameObject customNightButtonLoc;
    [SerializeField] private GameObject customNightButton;
    [SerializeField] private GameObject customNightBackButton;
    [SerializeField] private GameObject customNightReadyButton;
    [SerializeField] private GameObject pointer;

    [SerializeField] private GameObject[] stars;

    [SerializeField] private AILevel[] aiLevels;

    private bool deleteHeld = false;
    private bool didDelete = false;
    private float deleteTime = 0.0f;
    private float deleteWhenHeldFor = 1.0f;

    private int progress = 0;
    private bool beatGame = false;
    private bool beatSix = false;
    private bool beatCustom = false;
    private bool playerHasControl = true;

    private float twitchTimer = 0.0f;

    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

        //1 in 1000 chance for the eyeless bonnie screen on loading the main menu
        if (Random.Range(0, 1000) == 0 && !TempData.playerWon) {
            SetBonnieScreenStart();
        }

        SwitchToCamera(MenuCamera.Main_Menu);

        progress = PlayerPrefs.GetInt("Progress");

        if (TempData.playerWon) { //Player loaded into main menu after having beaten the night
            TempData.playerWon = false;
            if (TempData.loadNight < 5) {
                progress = TempData.loadNight + 1;
                PlayerPrefs.SetInt("Progress", progress);
                DoContinueGame();
            } else if (TempData.loadNight == 5) {
                PlayerPrefs.SetInt("BeatGame", 1);
                hiderPlane.SetActive(true);
                PlayStoryBeat(StoryBeat.Beat_Game);
            } else if (TempData.loadNight == 6) {
                PlayerPrefs.SetInt("Beat6", 1);
                hiderPlane.SetActive(true);
                PlayStoryBeat(StoryBeat.Beat_6);
            } else if (TempData.loadNight == 7 && TempData.bonnieAI == 20 && TempData.chicaAI == 20 && TempData.freddyAI == 20 && TempData.foxyAI == 20) {
                PlayerPrefs.SetInt("Beat420", 1);
                hiderPlane.SetActive(true);
                PlayStoryBeat(StoryBeat.Beat_420);
            } else {
                menuMusic.Play();
                staticAudio.Play();
            }
        } else if (TempData.playStoryBeat != (int)StoryBeat.None) { //Player loaded into main menu after having watched a story beat
            if (TempData.playStoryBeat == (int)StoryBeat.New_Game) { //Player watched the new game cutscene
                TempData.playStoryBeat = (int)StoryBeat.None;
                DoContinueGame(); //Move onto the game
                return;
            } else {
                TempData.playStoryBeat = (int)StoryBeat.None;
                menuMusic.Play();
                staticAudio.Play();
            }
        } else { //Player loaded into main menu for a reason besides beating a night or having watched a story beat.
            menuMusic.Play();
            staticAudio.Play();
        }

        beatGame = PlayerPrefs.GetInt("BeatGame") == 1;
        beatSix = PlayerPrefs.GetInt("Beat6") == 1;
        beatCustom = PlayerPrefs.GetInt("Beat420") == 1;
        stars[0].SetActive(beatGame);
        stars[1].SetActive(beatSix);
        stars[2].SetActive(beatCustom);

        if (beatSix) {
            latestItem = Item.Custom_Night;
        } else if (beatGame) {
            latestItem = Item.Night_Six;
        } else if (progress > 0) {
            latestItem = Item.Continue;
        } else {
            latestItem = Item.New_Game;
        }

        continueButtonLoc.SetActive(progress > 0);
        nightSixButtonLoc.SetActive(beatGame);
        customNightButtonLoc.SetActive(beatSix);

        UpdatePointer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerHasControl) {
            return;
        }

        //Where the mouse is pointing
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Upon clicking enter (return key)
        if (Input.GetKeyDown(KeyCode.Return)) {
            switch (currentItem)
            {
                case (Item.New_Game):
                    DoNewGame();
                    break;
                case (Item.Continue):
                    DoContinueGame();
                    break;
                case (Item.Night_Six):
                    DoNightSix();
                    break;
                case (Item.Custom_Night):
                    DoCustomNight();
                    break;
                default:
                    break;
            }
        }

        //Upon clicking down key
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentItem == latestItem) {
                currentItem = Item.New_Game;
            } else {
                currentItem = (Item)((int)currentItem + 1);
            }
            UpdatePointer();
        }

        //Upon clicking up key
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentItem == Item.New_Game) {
                currentItem = latestItem;
            } else {
                currentItem = (Item)((int)currentItem - 1);
            }
            UpdatePointer();
        }

        //Upon clicking something
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //click the "New Game" GameObject
            if (hit.collider.name == newGameButton.name) {
                DoNewGame();
            }

            //click the "Continue" GameObject
            else if (hit.collider.name == continueButton.name) {
                DoContinueGame();
            }

            //click the "6th Night" GameObject
            else if (hit.collider.name == nightSixButton.name) {
                DoNightSix();
            }

            //click the "Custom Night" GameObject
            else if (hit.collider.name == customNightButton.name) {
                DoCustomNight();
            }

            //click the back button on the custom night screen
            else if (hit.collider.name == customNightBackButton.name) {
                SwitchToCamera(MenuCamera.Main_Menu);
            }

            //click the ready button on the custom night screen
            else if (hit.collider.name == customNightReadyButton.name) {
                DoNightSeven();
            }
        }

        //Upon hovering something, move the pointer
        if (Physics.Raycast(ray, out hit))
        {
            //"New Game" GameObject
            if (hit.collider.name == newGameButton.name) {
                currentItem = Item.New_Game;
            }

            //"Continue" GameObject
            if (hit.collider.name == continueButton.name) {
                currentItem = Item.Continue;
            }

            //"6th Night" GameObject
            if (hit.collider.name == nightSixButton.name) {
                currentItem = Item.Night_Six;
            }

            //"Custom Night" GameObject
            if (hit.collider.name == customNightButton.name) {
                currentItem = Item.Custom_Night;
            }

            UpdatePointer();
        }

        //----------------Aesthetic stuff------------------
        //Freddy twitching
        twitchTimer += Time.deltaTime;
        if (twitchTimer >= 8.0f / 100.0f) {
            twitchTimer -= 8.0f / 100.0f;
            anim.SetInteger("MenuHallucination", Random.Range(0, 100));
        }

        //Delete progress upon holding Delete for the duration of 1 second
        deleteHeld = Input.GetKey(KeyCode.Delete);
        
        if (deleteHeld && !didDelete) {
            deleteTime += Time.deltaTime;
            if (deleteTime >= deleteWhenHeldFor) {
                DeleteProgress();
                didDelete = true;
            }
        } else if (deleteHeld && didDelete) {
            //Sit here until the player stops holding delete
        }
        else { 
            didDelete = false;
            deleteTime = 0.0f;
        }
    }

    public void SetBonnieScreenComplete()
    {
        anim.SetBool("Bonnie", false);
        transform.position = transform.position + new Vector3(0, -2, 0);
        menuItemsContainer.SetActive(true);
    }

    public void SetBonnieScreenStart()
    {
        anim.SetBool("Bonnie", true);
        transform.position = transform.position + new Vector3(0, 2, 0);
        menuItemsContainer.SetActive(false);
    }

    private void DoWarningMessage()
    {
        anim.SetBool("Warning", true);
        transform.position = transform.position + new Vector3(0, 2, 0);
        menuItemsContainer.SetActive(false);
    }

    private void DoNewGame()
    {
        TempData.loadNight = 1;
        PlayerPrefs.SetInt("Progress", TempData.loadNight);
        PlayStoryBeat(StoryBeat.New_Game);
    }

    private void DoContinueGame()
    {
        TempData.loadNight = progress;
        StartNight();
    }

    private void DoNightSix()
    {
        TempData.loadNight = 6;
        StartNight();
    }

    private void DoNightSeven()
    {
        if (aiLevels[0].GetLevel() == 1
        && aiLevels[1].GetLevel() == 9
        && aiLevels[2].GetLevel() == 8
        && aiLevels[3].GetLevel() == 7) 
        {
            SceneManager.LoadScene("Golden Freddy");
        } else {
            TempData.loadNight = 7;
            StartNight();
        }
    }

    private void DoCustomNight()
    {
        SwitchToCamera(MenuCamera.Custom_Night);
    }

    private void StartNight()
    {
        playerHasControl = false;

        if (TempData.loadNight != 7) { //Night 7 is custom night
            TempData.freddyAI = AILevels[TempData.loadNight - 1, 0];
            TempData.bonnieAI = AILevels[TempData.loadNight - 1, 1];
            TempData.chicaAI = AILevels[TempData.loadNight - 1, 2];
            TempData.foxyAI = AILevels[TempData.loadNight - 1, 3];
        } else {
            TempData.freddyAI = aiLevels[0].GetLevel();
            TempData.bonnieAI = aiLevels[1].GetLevel();
            TempData.chicaAI = aiLevels[2].GetLevel();
            TempData.foxyAI = aiLevels[3].GetLevel();
        }

        //On night 4 Freddy has a chance to be slightly more difficult
        if (TempData.loadNight == 4 && Random.Range(0, 2) == 0) {
            TempData.freddyAI += 1;
        }

        SwitchToCamera(MenuCamera.Night_Fade);

        Messenger.Broadcast(GameEvent.START_NIGHT);
    }

    private void SwitchToCamera(MenuCamera camera)
    {
        for (int i = 0; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(false);
        }

        cameras[(int)camera].gameObject.SetActive(true);
    }

    private void UpdatePointer()
    {
        continueNightText.SetActive(currentItem == Item.Continue);
        continueNightNumber.SetInteger("Progress", progress);
        switch (currentItem)
        {
            case (Item.New_Game):
                pointer.transform.position = newGameButtonLoc.transform.position;
                return;
            case (Item.Continue):
                pointer.transform.position = continueButtonLoc.transform.position;
                return;
            case (Item.Night_Six):
                pointer.transform.position = nightSixButtonLoc.transform.position;
                return;
            case (Item.Custom_Night):
                pointer.transform.position = customNightButtonLoc.transform.position;
                return;
            default:
                return;
        }
    }

    void PlayStoryBeat(StoryBeat beat) {
        TempData.playStoryBeat = (int)beat;
        if (beat == StoryBeat.New_Game) {
            StartCoroutine("TakeScreenshotAndChangeScene");
        } else {
            SceneManager.LoadScene("Story Beat");
        }
    }

    void DeleteProgress() {
        blipClip.Play();
        PlayerPrefs.SetInt("Progress", 0);
        PlayerPrefs.SetInt("BeatGame", 0);
        PlayerPrefs.SetInt("Beat6", 0);
        PlayerPrefs.SetInt("Beat420", 0);

        progress = 0;
        beatGame = false;
        beatSix = false;
        beatCustom = false;

        continueButtonLoc.SetActive(false);
        nightSixButtonLoc.SetActive(false);
        customNightButtonLoc.SetActive(false);

        latestItem = Item.New_Game;

        for (int i = 0; i < stars.Length; i++) {
            stars[i].SetActive(false);
        }

        currentItem = Item.New_Game;

        UpdatePointer();
    }

    private IEnumerator TakeScreenshotAndChangeScene()
    {
        Camera _camera = cameras[0];
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
        TempData.screenshot = tempSprite;
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene("Story Beat");
    }
}
