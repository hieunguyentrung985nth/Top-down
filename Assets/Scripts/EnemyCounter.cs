using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public static EnemyCounter Instance { get; private set; }

    private int totalEnemies = 0;

    private bool isPlayingInGameMusic = false;

    public event Action OnPlayInGameMusic;


    private void Awake()
    {
        Instance = this;
    }

    public void IncreaseEnemy()
    {
        totalEnemies += 1;
    }

    public void DecreaseEnemy()
    {
        totalEnemies -= 1;

        if (!isPlayingInGameMusic)
        {
            isPlayingInGameMusic = true;

            OnPlayInGameMusic?.Invoke();
        }

    }

    public bool CanPlayInGameMusic()
    {
        return isPlayingInGameMusic;
    }

    public void SetCanPlayInGameMusic(bool value)
    {
        isPlayingInGameMusic = value;
    }

    public int GetTotalEnemies()
    {
        return totalEnemies;
    }

    public void ResetTotalEnemies()
    {
        totalEnemies = 0;
    }

    public void PlayerDead()
    {
        LightSystem.Instance.SetLight(1f, Color.red);

        HandleScenes.Instance.SurvivalModeEnd();

        //OnSurvivalModeEndEvent?.Invoke(); 
    }
}
