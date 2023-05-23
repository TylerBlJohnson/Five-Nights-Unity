using UnityEngine; //For the Sprite variable type

public static class TempData
{
    public static int loadNight { get; set; }   //Used to determine which night to load, and which night the player beat after returning to the main menu (used in tandem with playerWon)
    public static int bonnieAI { get; set; }    //Bonnie's AI level, passed from the main menu
    public static int chicaAI { get; set; }     //Chica's AI level, passed from the main menu
    public static int freddyAI { get; set; }    //Freddy's AI level, passed from the main menu
    public static int foxyAI { get; set; }      //Foxy's AI level, passed from the main menu
    public static GameController.CameraMonitor playerViewingCamera { get; set; } //Used in various locations to determine which camera the player is monitoring
    public static bool leftDoorDown { get; set; }       //The current status of the left door
    public static bool rightDoorDown { get; set; }      //The current status of the right door
    public static bool lightOn { get; set; }            //Whether one of the lights in the office are on
    public static bool dying { get; set; }              //Set at the start of a jumpscare to prevent various things from happening
    public static bool playerWon { get; set; }          //Set at the start of the win sequence to prevent various things from happening. Used by the main menu to determine player progress
    public static int foxyPrep { get; set; }            //Used in GameController and Animatronics interchangeably to determine what Foxy is doing
    public static bool camerasDisabled { get; set; }    //Used across the various cameras to determine whether or not the camera is disabled. Is set when an animatronic moves while the player is looking at them.
    public static int playStoryBeat { get; set; }       //Used to play a short one-image cutscene before or after finishing a night.
    public static Sprite screenshot { get; set; }       //Used exclusively to apply an image of the main menu to the newspaper clip cutscene.
    public static bool hallucinationOverlayPlaying { get; set; } //Used to control volume of the robotvoice
    public static bool hallucinationOverlayVisible { get; set; }    //Used to sync the hallucination overlay visibility.
    public static int hallucinationOverlayValue { get; set; }       //Used to sync the hallucination overlay values.
    public static bool manuallyHallucinate { get; set; }            //Used to force the hallucination overlay to play.
    public static int remainingPower { get; set; }              //The player's remaining power as seen in the lower left corner of the screen. 0-999. Display truncates the final digit.
    public static PowerOut.PowerState powerState { get; set; }  //The current power state. Goes Power_On, Power_Off, Freddy_Music, Flicker_Off, Await_Death, and Jumpscare. In that order.
    public static bool didFullscreenSwitch { get; set; }    //Used to allow the player to switch between fullscreen and windowed mode smoothly.
    public static int windowedWidth { get; set; }           //Used to allow the player to switch between fullscreen and windowed mode smoothly.
    public static int windowedHeight { get; set; }          //Used to allow the player to switch between fullscreen and windowed mode smoothly.
}