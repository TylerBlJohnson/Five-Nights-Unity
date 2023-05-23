using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private enum LookDirection
    {
        Left,
        Straight,
        Right
    }
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject leftPlane;
    [SerializeField] private GameObject straightPlane;
    [SerializeField] private GameObject rightPlane;
    [SerializeField] private GameObject monitorUpPlane;
    [SerializeField] private GameObject monitorDownPlane;
    private bool cameraIsDown = true;
    private bool monitorCanFlip = true;
    private float officeLookSpeed = 9.0f;
    private bool forceCameraLeft = false;
    private LookDirection lookDirection = LookDirection.Straight;
    private Ray ray;
    private RaycastHit hit;

    void Awake()
    {
        Messenger.AddListener(GameEvent.FORCE_CAMERA_LEFT, OnForceCameraLeft); //Used to force the camera towards Foxy's jumpscare
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.FORCE_CAMERA_LEFT, OnForceCameraLeft);
    }

    // Update is called once per frame
    void Update()
    {
        //Where the mouse is pointing
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the mouse is pointing at something
        if(Physics.Raycast(ray, out hit))
        {
            //Only move the camera if the camera is down
            if (cameraIsDown) {
                //Tried to use a switch case here but was told no because "a constant value is expected". This works as is so deal with it.
                //if the mouse is pointing at the "leftPlane" GameObject
                if (hit.collider.name == leftPlane.name) {
                    lookDirection = LookDirection.Left;
                    monitorCanFlip = true;
                }

                //if the mouse is pointing at the "rightPlane" GameObject
                else if (hit.collider.name == rightPlane.name) {
                    lookDirection = LookDirection.Right;
                    monitorCanFlip = true;
                }

                //if the mouse is pointing at the "straightPlane" GameObject
                else if (hit.collider.name == straightPlane.name) {
                    lookDirection = LookDirection.Straight;
                    monitorCanFlip = true;
                }

                else if (hit.collider.name == monitorUpPlane.name) {
                    if (monitorCanFlip) {
                        lookDirection = LookDirection.Straight;
                        monitorCanFlip = false;
                        Messenger.Broadcast(GameEvent.CAMERA_FLIP);
                    }
                }

                else if (hit.collider.name == monitorDownPlane.name) {
                    if (monitorCanFlip) {
                        monitorCanFlip = false;
                        Messenger.Broadcast(GameEvent.SWITCH_TO_OFFICE);
                    }
                }

                else {
                    monitorCanFlip = true;
                }
            }
        }

        if (forceCameraLeft && TempData.remainingPower > 0) {
            lookDirection = LookDirection.Left;
        } 

        Transform cameraPosition = mainCamera.transform;
        switch(lookDirection) {
            case(LookDirection.Left):
                cameraPosition.Translate(officeLookSpeed * Time.deltaTime, 0, 0);
                break;
            
            case(LookDirection.Right):
                cameraPosition.Translate(-officeLookSpeed * Time.deltaTime, 0, 0);
                break;
        }

        //if the camera is too far left, move it back
        if (cameraPosition.position.x > 2.2f) {
            cameraPosition.position = new Vector3(2.2f, cameraPosition.position.y, cameraPosition.position.z);
        }

        //if the camera is too far right, move it back
        if (cameraPosition.position.x < -2.2f) {
            cameraPosition.position = new Vector3(-2.2f, cameraPosition.position.y, cameraPosition.position.z);
        }

        //if the power outage Freddy jumpscare is playing, keep the camera centered
        if (TempData.powerState == PowerOut.PowerState.Jumpscare) {
            cameraPosition.position = new Vector3(0, cameraPosition.position.y, cameraPosition.position.z);
        }
    }

    private void OnForceCameraLeft()
    {
        forceCameraLeft = true;
    }
}
