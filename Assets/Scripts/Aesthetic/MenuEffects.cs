using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEffects : MonoBehaviour
{
    private Animator anim;
    private Renderer rend;
    private float R;
    private float G;
    private float B;

    private float timeSinceLastTransparencyUpdate = 0.0f;
    private float transparencyUpdate = 8.0f / 100.0f;

    private float timeSinceLastScanLineUpdate = 0.0f;
    private float scanLineUpdate = 30.0f / 100.0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
        R = rend.material.color.r;
        G = rend.material.color.g;
        B = rend.material.color.b;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastTransparencyUpdate += Time.deltaTime;
        timeSinceLastScanLineUpdate += Time.deltaTime;

        if (timeSinceLastTransparencyUpdate >= transparencyUpdate) {
            timeSinceLastTransparencyUpdate -= transparencyUpdate;
            rend.material.color = new Color(R, G, B, Random.Range(100, 200) / 255.0f);
        }

        if (timeSinceLastScanLineUpdate >= scanLineUpdate) {
            timeSinceLastScanLineUpdate -= scanLineUpdate;
            anim.SetInteger("Effect", Random.Range(0, 9));
            anim.SetBool("Visible", Random.Range(0, 3) == 0);
        }
    }
}
