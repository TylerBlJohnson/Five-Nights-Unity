using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScanline : MonoBehaviour
{
    private Renderer rend;
    private float R;
    private float G;
    private float B;

    private float timeSinceLastTransparencyUpdate = 0.0f;
    private float transparencyUpdate = 8.0f / 100.0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        R = rend.material.color.r;
        G = rend.material.color.g;
        B = rend.material.color.b;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastTransparencyUpdate += Time.deltaTime;

        if (timeSinceLastTransparencyUpdate >= transparencyUpdate) {
            timeSinceLastTransparencyUpdate -= transparencyUpdate;
            rend.material.color = new Color(R, G, B, Random.Range(100, 200) / 255.0f);
        }
    }
}
