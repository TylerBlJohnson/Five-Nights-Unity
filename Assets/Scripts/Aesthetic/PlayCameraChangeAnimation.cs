using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraChangeAnimation : MonoBehaviour
{
    void Awake()
    {
        Messenger.AddListener(GameEvent.SWITCH_TO_MONITOR, PlayAnimation);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHANGE_CAMERA, PlayAnimation);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.SWITCH_TO_MONITOR, PlayAnimation);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHANGE_CAMERA, PlayAnimation);
    }

    void PlayAnimation()
    {
        if (Random.Range(0,2) == 0) {
            gameObject.GetComponent<Animator>().SetTrigger("CamChange1");
        } else {
            gameObject.GetComponent<Animator>().SetTrigger("CamChange2");
        }
    } 

    void PlayAnimation(GameController.CameraMonitor monitor)
    {
        PlayAnimation();
    }
}
