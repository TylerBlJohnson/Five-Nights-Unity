using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityScript : MonoBehaviour
{
    private enum LookDirection
    {
        Left,
        Right
    }

    [SerializeField] private AudioSource cameraSound;
    
    private float panSpeed = 1.0f;
    private float panTime = 0.0f;
    private static float howLongToPan = 7.0f;
    private LookDirection lookDirection = LookDirection.Left;
    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        //Move the camera over to the right side so it doesn't get stuck on the left for an extended period of time at the start.
        transform.position = new Vector3(-2.2f, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        //Where the mouse is pointing
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the mouse is pointing at something
        if(Physics.Raycast(ray, out hit))
        {
            //if the mouse is pointing at the "leftPlane" GameObject
            // if (hit.collider.name == leftPlane.name) {
            //     lookDirection = LookDirection.Left;
            // }
        }

        panTime += Time.deltaTime;

        if (panTime >= howLongToPan) {
            panTime = 0.0f;

            if (lookDirection == LookDirection.Left) {
                lookDirection = LookDirection.Right;
            } else {
                lookDirection = LookDirection.Left;
                try
                {
                    if (!TempData.playerWon && !TempData.dying){
                        cameraSound.Play();
                    }
                }
                catch (System.Exception){}
                
            }
        }

        try
        {
            if (TempData.playerViewingCamera == GameController.CameraMonitor.Office) {
                cameraSound.volume = 0.0f;
            } else {
                cameraSound.volume = 1.0f;
            }
        } catch (System.Exception){}
        

        switch(lookDirection) {
            case(LookDirection.Left):
                transform.Translate(-panSpeed * Time.deltaTime, 0, 0);
                break;
            
            case(LookDirection.Right):
                transform.Translate(panSpeed * Time.deltaTime, 0, 0);
                break;
        }

        //if the camera is too far left, move it back
        if (transform.position.x > 2.2f) {
            transform.position = new Vector3(2.2f, transform.position.y, transform.position.z);
        }

        //if the camera is too far right, move it back
        if (transform.position.x < -2.2f) {
            transform.position = new Vector3(-2.2f, transform.position.y, transform.position.z);
        }
    }
}
