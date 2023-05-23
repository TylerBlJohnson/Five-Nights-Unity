using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force16By9 : MonoBehaviour
{
    /*
    void Start () 
    {
        //Debug.Log(Screen.resolutions);

        foreach(var res in Screen.resolutions) {
            Debug.Log(res.width + "x" + res.height);
        }
    }
    //*/

    

    //*
    // Update is called once per frame
    void Update()
    {
        if (!TempData.didFullscreenSwitch && ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKey(KeyCode.Return))) {
            //Debug.Log("Fullscreen switch detected");
            TempData.didFullscreenSwitch = true;
            if (!Screen.fullScreen) {
                TempData.windowedWidth = Screen.width;
                TempData.windowedHeight = Screen.height;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            } else {
                if (TempData.windowedWidth > 0) {
                    Screen.SetResolution(TempData.windowedWidth, TempData.windowedHeight, false);
                } else {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
                }
            }
        } else if (TempData.didFullscreenSwitch && !((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKey(KeyCode.Return))) {
            TempData.didFullscreenSwitch = false;
        }

        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {  
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
            
            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
    //*/
}
