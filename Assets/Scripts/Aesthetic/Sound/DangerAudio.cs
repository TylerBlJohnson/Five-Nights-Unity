using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Thanks random YT commenter Yqe on YT video https://www.youtube.com/watch?v=t3BVB_s8T0U
"There's actually another way to kind of keep track of the animatronics based on sound. You know the alarm ambience thingy? It has very 
specific conditions for it to play, and the volume also changes based on it. 

The things that triggers it is if foxy is above stage 2 (outside pirate cove), bonnie is in the west hallway or closet (this includes being 
outside your door and in the corner and etc), and if chica is in the east hallway (and outside window etc). If one of these 3 conditions are 
met, the sound plays at 30%, if 2 conditions are met, it plays at 50%, and if 3 conditions are met, it plays at 75%. Kind of hard to hear the 
individual volume but it's still useful for this.

Oh, and there's also a fourth separate conditions, which is if freddy is inside your office. This causes the sound to play at 100%, but you're 
probably already screwed if this happens so it shouldn't matter for the other stuff."
*/


public class DangerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource dangerAudio;
    private bool bonnieDanger = false;
    private bool chicaDanger = false;
    private bool freddyDanger = false;
    private bool foxyDanger = false;

    void Awake()
    {
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHICA_MOVED, OnChicaMoved);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.FREDDY_MOVED, OnFreddyMoved);
        Messenger.AddListener(GameEvent.FOXY_MOVED, OnFoxyMoved);
    }

    void OnDestroy()
    {
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHICA_MOVED, OnChicaMoved);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.FREDDY_MOVED, OnFreddyMoved);
        Messenger.RemoveListener(GameEvent.FOXY_MOVED, OnFoxyMoved);
    }

    private void OnBonnieMoved(GameController.CameraMonitor monitor) 
    {
        if (monitor == GameController.CameraMonitor.West_Hall 
        || monitor == GameController.CameraMonitor.West_Hall_Corner
        || monitor == GameController.CameraMonitor.Supply_Closet
        || monitor == GameController.CameraMonitor.Outside_Office
        || monitor == GameController.CameraMonitor.Office) {
            bonnieDanger = true;
        } else {
            bonnieDanger = false;
        }

        //Debug.Log("Bonnie danger is : " + bonnieDanger);

        UpdateVolume();
    }

    private void OnChicaMoved(GameController.CameraMonitor monitor) 
    {
        if (monitor == GameController.CameraMonitor.East_Hall 
        || monitor == GameController.CameraMonitor.East_Hall_Corner
        || monitor == GameController.CameraMonitor.Outside_Office
        || monitor == GameController.CameraMonitor.Office) {
            chicaDanger = true;
        } else {
            chicaDanger = false;
        }

        //Debug.Log("Chica Danger is: " + chicaDanger);

        UpdateVolume();
    }

    private void OnFreddyMoved(GameController.CameraMonitor monitor)
    {
        if (monitor == GameController.CameraMonitor.Office) {
            freddyDanger = true;
        }

        UpdateVolume();
    }

    private void OnFoxyMoved() 
    {
        //foxyPrep is 2 once he's fully outside of pirate cove but hasn't left yet
        foxyDanger = TempData.foxyPrep >= 2;

        //Debug.Log("Foxy danger is " + foxyDanger);

        UpdateVolume();
    }

    private void UpdateVolume() 
    {
        int dangerLevel = 0;

        if (bonnieDanger) {
            dangerLevel++;
        }

        if (chicaDanger) {
            dangerLevel++;
        }

        if (foxyDanger) {
            dangerLevel++;
        }

        //Max out danger level if Freddy gets in the office
        if (freddyDanger) {
            dangerLevel = 4;
        }

        if (TempData.playerWon || TempData.dying) {
            dangerLevel = 0;
        }

        //Debug.Log("Danger level: " + dangerLevel);

        switch (dangerLevel)
        {
            case(0):
                dangerAudio.volume = 0.00f;
                break;
            case(1):
                dangerAudio.volume = 0.30f;
                break;
            case(2):
                dangerAudio.volume = 0.50f;
                break;
            case(3):
                dangerAudio.volume = 0.75f;
                break;
            case(4):
                dangerAudio.volume = 1.00f;
                break;
            default:
                Debug.LogError("Too much danger! Maximum should be 4, instead danger level is " + dangerLevel);
                break;
        }
    }
}
