using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlam : MonoBehaviour
{
    [SerializeField] private AudioSource doorSlam;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void PlayDoorSlam()
    {
        doorSlam.Play();
    }
}
