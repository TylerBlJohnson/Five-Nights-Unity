using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private AudioSource jumpscare;

    private void DoStartJumpscare() {
        Messenger.Broadcast(GameEvent.JUMPSCARE);
        Messenger.Broadcast(GameEvent.SWITCH_TO_OFFICE);
        jumpscare.Play();
    }

    private void DoGameOver() {
        if (!TempData.playerWon) { //Player is not allowed to lose in the middle of winning.
            Messenger.Broadcast(GameEvent.GAME_OVER);
        }
    }
}
