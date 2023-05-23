using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    private enum BackgroundState {
        Inactive,
        Active
    }

    [SerializeField] private GameController.CameraMonitor thisMonitor;
    [SerializeField] private bool isActive = false;
    [SerializeField] private SpriteRenderer text;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite[] cameraTextTextures;
    [SerializeField] private Sprite[] backgroundTextures;

    void Start()
    {
        if (thisMonitor == GameController.CameraMonitor.Office) {
            Debug.LogError("Camera monitor cannot be set to Office");
        } else {
            //Assign sprite to camera tile text
            text.sprite = cameraTextTextures[(int)thisMonitor - 1]; //Subtract 1 because the text textures array does not include the office.

            if (backgroundTextures.Length > 0) {
                if (isActive) {
                    background.sprite = backgroundTextures[(int)BackgroundState.Active];
                } else {
                    background.sprite = backgroundTextures[(int)BackgroundState.Inactive];
                }
            }
        }
    }

    //When clicked
    void OnMouseDown()
    {
        if (!isActive) {
            Messenger<GameController.CameraMonitor>.Broadcast(GameEvent.CHANGE_CAMERA, thisMonitor);
        }
    }
}
