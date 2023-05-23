using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles hallucination behavior by rolling a new random number each time the player switches cameras.
public class Hallucination : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        anim.SetInteger("HallucinationValue", Random.Range(0, 1000));
    }

    void Awake()
    {
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger.AddListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
    }

    void OnDestroy()
    {
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger.RemoveListener(GameEvent.SWITCH_TO_OFFICE, OnSwitchToOffice);
    }
    
    private void OnChangeCamera(GameController.CameraMonitor _) 
    {
        updateHallucination();
    }

    private void OnSwitchToOffice()
    {
        updateHallucination();
    }

    private void updateHallucination()
    {
        anim.SetInteger("HallucinationValue", Random.Range(0, 1000));
    }
}
