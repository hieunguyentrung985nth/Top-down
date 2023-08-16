using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBossSurvival : MonoBehaviour
{
    public event Action OnWaveBossDeadEvent;

    public void WaveBossDead()
    {
        OnWaveBossDeadEvent?.Invoke();
    }
}
