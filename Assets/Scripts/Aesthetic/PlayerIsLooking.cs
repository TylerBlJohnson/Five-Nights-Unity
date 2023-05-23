using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsLooking : MonoBehaviour
{
    [SerializeField] GameController.CameraMonitor thisMonitor;
    GameController.CameraMonitor viewingMonitor;
    private Animator animator;

    void Awake()
    {
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger.AddListener(GameEvent.SWITCH_TO_MONITOR, OnSwitchToMonitor);
    }

    void OnDestroy()
    {
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHANGE_CAMERA, OnChangeCamera);
        Messenger.RemoveListener(GameEvent.SWITCH_TO_MONITOR, OnSwitchToMonitor);
    }

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnChangeCamera(GameController.CameraMonitor monitor)
    {
        viewingMonitor = monitor;
        SetIsPlayerLooking(thisMonitor == viewingMonitor);
    }

    void OnSwitchToMonitor()
    {
        SetIsPlayerLooking(thisMonitor == viewingMonitor);
    }

    void SetIsPlayerLooking(bool isViewing) 
    {
        animator.SetBool("PlayerViewing", isViewing);
    }
}
