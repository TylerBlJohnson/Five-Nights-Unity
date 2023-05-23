using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the backstage behavior where Bonnie will move close to the camera if the player lowers the monitor while Bonnie is there
//Alternatively, Bonnie may move close to the camera 10% of the time regardless of the above
public class BackstageBonnie : MonoBehaviour
{
    private GameController.CameraMonitor playerMonitor = GameController.CameraMonitor.Office;
    private GameController.CameraMonitor bonnieMonitor = GameController.CameraMonitor.Show_Stage;
    private Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    void Awake()
    {
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger.AddListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
    }

    void OnDestroy()
    {
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger.RemoveListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
    }

    private void OnBonnieMoved(GameController.CameraMonitor monitor) 
    {
        bonnieMonitor = monitor;

        anim.SetBool("BonnieHallucination", Random.Range(0, 10) == 0);
    }

    private void OnChangeCamera(GameController.CameraMonitor monitor) 
    {
        playerMonitor = monitor;
    }

    private void OnSwitchToOffice() 
    {
        anim.SetBool("BonnieHallucination", bonnieMonitor == playerMonitor);
    }
}
