using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInventory : MonoBehaviour
{
    private DataToSave data = new DataToSave();

    private PlayerStats playerStats;

    [SerializeField] private InventoryObject inventory;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        //HandleScenes.Instance.OnGameStartEvent += Instance_OnGameStartEvent;

        HandleScenes.Instance.OnGameEndEvent += Instance_OnGameEndEvent;

        gameManager.OnSaveGameEvent += GameManager_OnSaveGameEvent;
    }

    private void Instance_OnGameEndEvent()
    {
        SaveData();
    }

    private void GameManager_OnSaveGameEvent()
    {
        SaveData();
    }

    public void SaveData()
    {
        inventory.Container.weapons = playerStats.GetPlayerWeapons();

        inventory.Container.info.money = playerStats.GetPlayerMoney();

        inventory.Container.power = playerStats.GetPlayerPowers();

        inventory.Container.info.currentLevel = gameManager.GetCurrentLevel();

        inventory.Container.info.characterIndex = gameManager.GetCurrentCharacterIndex();

        inventory.SavePlayer(inventory);
    }
}
