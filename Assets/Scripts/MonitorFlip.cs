using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorFlip : MonoBehaviour
{
    public enum Monitor_State
    {
        Down,
        Flipping_up,
        Up,
        Flipping_down
    }


    private Monitor_State monitorState = Monitor_State.Down;
    private Animator anim;

    [SerializeField] private AudioSource flipUp;
    [SerializeField] private AudioSource flipDown;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Monitor State", (int)monitorState);
    }

    private void SetMonitorState(Monitor_State state)
    {
        if (!TempData.playerWon) {
            if (monitorState != state) {
                monitorState = state;
                anim.SetInteger("Monitor State", (int)monitorState);

                switch (monitorState)
                {
                    case(Monitor_State.Up):
                        Messenger.Broadcast(GameEvent.SWITCH_TO_MONITOR);
                        break;
                    case(Monitor_State.Flipping_up):
                        flipUp.Play();
                        flipDown.Stop();
                        break;
                    case(Monitor_State.Flipping_down):
                        flipUp.Stop();
                        flipDown.Play();
                        break;
                    case(Monitor_State.Down):
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
