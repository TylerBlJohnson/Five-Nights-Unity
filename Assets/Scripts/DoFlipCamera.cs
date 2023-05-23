using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoFlipCamera : MonoBehaviour
{
    [SerializeField] private Animator monitorAnim;

    void Awake()
    {
        Messenger.AddListener(GameEvent.CAMERA_FLIP, OnCameraFlip);
        Messenger.AddListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.CAMERA_FLIP, OnCameraFlip);
        Messenger.RemoveListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
    }

    private void OnCameraFlip()
    {
        monitorAnim.SetTrigger("Open Camera");
    }

    private void OnSwitchToOffice()
    {
        monitorAnim.SetTrigger("Close Camera");
    }

    // Update is called once per frame
    void Update() 
    {
        if (TempData.playerWon) {
            monitorAnim.SetTrigger("Close Camera");
        }
    }
}
