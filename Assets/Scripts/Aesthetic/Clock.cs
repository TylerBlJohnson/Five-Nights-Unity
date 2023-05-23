using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private Animator[] clock;
    [SerializeField] private Animator calendar;

    void Awake()
    {
        Messenger<int>.AddListener(GameEvent.TIME_CHANGE, OnTimeChanged);
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.TIME_CHANGE, OnTimeChanged);
    }

    void Start()
    {
        calendar.SetInteger("Day", TempData.loadNight);
    }

    private void OnTimeChanged(int time)
    {
        clock[0].SetInteger("Time", time);
        clock[1].SetInteger("Time", time);
    }
}
