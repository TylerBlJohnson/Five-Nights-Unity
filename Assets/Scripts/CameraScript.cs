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
    [SerializeField] [Range(0f, 0.5f)] private float screenEdgeDeadzone = 0.1f; // Percentage of screen width on each side to ignore
    [SerializeField] private bool useLegacyControls = false; // Use the old invisible-plane-based camera movement
    [SerializeField] private bool useMobileControls = false; // Require click/tap to flip monitor instead of hover
    private bool cameraIsDown = true;
    private bool monitorCanFlip = true;
    private readonly float officeLookSpeed = 9.0f;
    private bool forceCameraLeft = false;
    private readonly float officeWidth = 2.05f; //How far to the left or right the camera can move, as a multiplier
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
                // Legacy controls: use invisible planes to control camera direction
                if (useLegacyControls) {
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
                }

                // Monitor flip detection - either hover or click based on mobile toggle
                bool shouldFlip = useMobileControls ? Input.GetMouseButtonDown(0) : monitorCanFlip;

                if (hit.collider.name == monitorUpPlane.name) {
                    if (shouldFlip) {
                        lookDirection = LookDirection.Straight;
                        monitorCanFlip = false;
                        Messenger.Broadcast(GameEvent.CAMERA_FLIP);
                    }
                }

                else if (hit.collider.name == monitorDownPlane.name) {
                    if (shouldFlip) {
                        monitorCanFlip = false;
                        Messenger.Broadcast(GameEvent.SWITCH_TO_OFFICE);
                    }
                }

                else {
                    monitorCanFlip = true;
                }
            }
        }

        Transform cameraPosition = mainCamera.transform;

        // Camera movement logic
        if (forceCameraLeft && TempData.remainingPower > 0) {
            // Forced camera movement overrides everything
            lookDirection = LookDirection.Left;
            
            cameraPosition.Translate(officeLookSpeed * officeWidth * Time.deltaTime, 0, 0);
            
            //if the camera is too far left, move it back
            if (cameraPosition.position.x > 2.2f * officeWidth) {
                cameraPosition.position = new Vector3(2.2f * officeWidth, cameraPosition.position.y, cameraPosition.position.z);
            }
        }
        else if (cameraIsDown && TempData.powerState != PowerOut.PowerState.Jumpscare) {
            if (useLegacyControls) {
                // Legacy controls: incremental movement based on lookDirection
                switch(lookDirection) {
                    case(LookDirection.Left):
                        cameraPosition.Translate(officeLookSpeed * officeWidth * Time.deltaTime, 0, 0);
                        break;
                    
                    case(LookDirection.Right):
                        cameraPosition.Translate(-officeLookSpeed * officeWidth * Time.deltaTime, 0, 0);
                        break;
                }

                //if the camera is too far left, move it back
                if (cameraPosition.position.x > 2.2f * officeWidth) {
                    cameraPosition.position = new Vector3(2.2f * officeWidth, cameraPosition.position.y, cameraPosition.position.z);
                }

                //if the camera is too far right, move it back
                if (cameraPosition.position.x < -2.2f * officeWidth) {
                    cameraPosition.position = new Vector3(-2.2f * officeWidth, cameraPosition.position.y, cameraPosition.position.z);
                }
            }
            else {
                // New controls: mouse-position-based camera movement
                // Get mouse X position normalized to 0-1 range
                float mouseXNormalized = Input.mousePosition.x / Screen.width;
                
                // Apply deadzone: remap the usable screen area (deadzone to 1-deadzone) to full camera range
                float usableScreenStart = screenEdgeDeadzone;
                float usableScreenEnd = 1f - screenEdgeDeadzone;
                
                // Clamp mouse position to usable area, then remap to 0-1
                float clampedMouseX = Mathf.Clamp(mouseXNormalized, usableScreenStart, usableScreenEnd);
                float remappedMouseX = (clampedMouseX - usableScreenStart) / (usableScreenEnd - usableScreenStart);
                
                // Map to camera position range: 0 = far left (+2.2 * officeWidth), 1 = far right (-2.2 * officeWidth)
                float targetX = Mathf.Lerp(2.2f * officeWidth, -2.2f * officeWidth, remappedMouseX);
                
                // Set camera position directly (no smoothing)
                cameraPosition.position = new Vector3(targetX, cameraPosition.position.y, cameraPosition.position.z);
            }
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