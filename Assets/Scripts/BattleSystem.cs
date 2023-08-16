using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private Transform currentLevelObject;

    private BattleTrigger battleTrigger;

    private GameObject spawner;

    private EnemySpawner[] spawnEnemy;

    private GameManager gameManager;

    private GameObject waves;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
       
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void GetBattleTrigger()
    {
        currentLevelObject = gameManager.GetCurrentLevelObject();

        spawner = currentLevelObject.gameObject;

        waves = spawner.transform.Find("Waves").gameObject;

        battleTrigger = spawner.transform.Find("Waves").GetChild(spawner.GetComponent<CampaignLevel>().currentWaveIndex).GetComponentInChildren<BattleTrigger>();

        battleTrigger.OnEnterBattle += BattleTrigger_OnEnterBattle;
    }

    public GameObject GetWaves()
    {
        return waves;
    }

    private void OnDisable()
    {
        if(battleTrigger != null)
            battleTrigger.OnEnterBattle -= BattleTrigger_OnEnterBattle;
    }

    private void BattleTrigger_OnEnterBattle(GameObject other)
    {
        gameManager.ChangeItemsOverTime();
    }

    
}
