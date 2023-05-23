using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenFreddyWHallCorner : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        anim.SetBool("GoldenFreddyIsPresent", false);
    }

    void Awake()
    {
        Messenger<Animatronics.GoldenFreddyStates>.AddListener(GameEvent.GOLDEN_FREDDY_UPDATE, OnGoldenFreddyUpdate);
    }

    void OnDestroy()
    {
        Messenger<Animatronics.GoldenFreddyStates>.RemoveListener(GameEvent.GOLDEN_FREDDY_UPDATE, OnGoldenFreddyUpdate);
    }

    private void OnGoldenFreddyUpdate(Animatronics.GoldenFreddyStates status)
    {
        anim.SetBool("GoldenFreddyIsPresent", status == Animatronics.GoldenFreddyStates.Poster || status == Animatronics.GoldenFreddyStates.Office);
    }
}
