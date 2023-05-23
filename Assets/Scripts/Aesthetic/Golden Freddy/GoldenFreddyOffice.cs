using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenFreddyOffice : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    void Awake()
    {
        Messenger<Animatronics.GoldenFreddyStates>.AddListener(GameEvent.GOLDEN_FREDDY_UPDATE, OnGoldenFreddyUpdate);
    }

    void OnDestroy()
    {
        Messenger<Animatronics.GoldenFreddyStates>.RemoveListener(GameEvent.GOLDEN_FREDDY_UPDATE, OnGoldenFreddyUpdate);
    }

    void Update()
    {
        if (TempData.dying || TempData.playerWon) {
            anim.SetBool("GoldenFreddyIsPresent", false);
        }
    }

    private void OnGoldenFreddyUpdate(Animatronics.GoldenFreddyStates status)
    {
        anim.SetBool("GoldenFreddyIsPresent", status == Animatronics.GoldenFreddyStates.Office);
    }
}
