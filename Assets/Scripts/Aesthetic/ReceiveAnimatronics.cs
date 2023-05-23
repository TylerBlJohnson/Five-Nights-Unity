using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveAnimatronics : MonoBehaviour
{
    [SerializeField] GameController.CameraMonitor thisMonitor;
    private Animator animator;
    private float disableTimer;
    private bool localCameraDisabled = false;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (thisMonitor == GameController.CameraMonitor.Show_Stage) {
            OnBonnieMoved(thisMonitor);
            OnChicaMoved(thisMonitor);
            OnFreddyMoved(thisMonitor);
        }
    }

    void Awake()
    {
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.CHICA_MOVED, OnChicaMoved);
        Messenger<GameController.CameraMonitor>.AddListener(GameEvent.FREDDY_MOVED, OnFreddyMoved);
        Messenger.AddListener(GameEvent.FOXY_MOVED, OnFoxyMoved);
        Messenger<bool>.AddListener(GameEvent.BONNIE_NEAR_CAMERA, OnBonnieMoved);
        Messenger<bool>.AddListener(GameEvent.CHICA_NEAR_CAMERA, OnChicaMoved);
        Messenger.AddListener(GameEvent.DISABLE_CAMERAS, OnDisableCameras);
    }

    void OnDestroy()
    {
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.BONNIE_MOVED, OnBonnieMoved);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.CHICA_MOVED, OnChicaMoved);
        Messenger<GameController.CameraMonitor>.RemoveListener(GameEvent.FREDDY_MOVED, OnFreddyMoved);
        Messenger.RemoveListener(GameEvent.FOXY_MOVED, OnFoxyMoved);
        Messenger<bool>.RemoveListener(GameEvent.BONNIE_NEAR_CAMERA, OnBonnieMoved);
        Messenger<bool>.RemoveListener(GameEvent.CHICA_NEAR_CAMERA, OnChicaMoved);
        Messenger.RemoveListener(GameEvent.DISABLE_CAMERAS, OnDisableCameras);
    }

    void OnBonnieMoved(GameController.CameraMonitor monitor)
    {
        animator.logWarnings = false;
        if (monitor == thisMonitor) {
            animator.SetBool("BonnieIsPresent", true);
        } else {
            animator.SetBool("BonnieIsPresent", false);
        }
        animator.logWarnings = true;
    }

    void OnBonnieMoved (bool bonnieNearCamera) {
        animator.logWarnings = false;
        animator.SetBool("BonnieNearCamera", bonnieNearCamera);
    }

    void OnChicaMoved(GameController.CameraMonitor monitor)
    {
        animator.logWarnings = false;
        if (monitor == thisMonitor) {
            animator.SetBool("ChicaIsPresent", true);
        } else {
            animator.SetBool("ChicaIsPresent", false);
        }
        animator.logWarnings = true;
    }

    void OnChicaMoved (bool chicaNearCamera) {
        animator.logWarnings = false;
        animator.SetBool("ChicaNearCamera", chicaNearCamera);
        animator.logWarnings = true;
    }

    void OnFreddyMoved(GameController.CameraMonitor monitor) {
        animator.logWarnings = false;
        if (monitor == thisMonitor) {
            animator.SetBool("FreddyIsPresent", true);
        } else {
            animator.SetBool("FreddyIsPresent", false);
        }
        animator.logWarnings = true;
    }

    void OnFoxyMoved() {
        animator.logWarnings = false;
        if (thisMonitor == GameController.CameraMonitor.Pirate_Cove) {
            animator.SetInteger("FoxyPrep", TempData.foxyPrep);
        } else if (thisMonitor == GameController.CameraMonitor.West_Hall) {
            animator.SetBool("FoxyIsPresent", TempData.foxyPrep >= 3);
        }
        animator.logWarnings = true;
    }

    void OnDisableCameras() {
        if (!localCameraDisabled) {
            StartCoroutine("DisableCameras");
        }
    }

    IEnumerator DisableCameras() {
        TempData.camerasDisabled = true;
        localCameraDisabled = true;
        float disableCamerasFor = 5.1f;

        animator.logWarnings = false;
        animator.SetBool("CameraDisabled", true);
        animator.logWarnings = true;
        for (float i = disableCamerasFor; i > 0; i -= Time.deltaTime) {
            yield return null;
        }
        animator.logWarnings = false;
        animator.SetBool("CameraDisabled", false);
        animator.logWarnings = true;
        TempData.camerasDisabled = false;
        localCameraDisabled = false;
    }
}
