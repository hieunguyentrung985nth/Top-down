using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLevel : MonoBehaviour
{
    [SerializeField][Range(1,3)] private int level;

    public int currentWaveIndex;

    public int totalWaves;

    private void Awake()
    {
        currentWaveIndex = 0;

        totalWaves = transform.childCount;
    }

    public int GetLevel()
    {
        return level;
    }   
}
