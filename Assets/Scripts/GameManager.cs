using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int totalLevels;

    private int currentLevel;

    public int totalEnemies;

    [SerializeField] private GameObject campaignMode;

    private Transform currentLevelObject;

    [SerializeField] private InventoryObject inventory;

    [SerializeField] private InventoryDatabase database;

    [Serializable]
    public class PhaseSpawnRate
    {
        public RandomSpawnRate[] rates;

        [Range(1,8)]
        public int level;
    }

    public enum GameState
    {
        IS_PLAYING,
        NOT_PLAYING
    }

    public GameState gameState;

    [SerializeField] private PhaseSpawnRate[] phase;

    public event Action<Transform> OnArrowEvent;

    private BattleSystem battleSystem;

    [SerializeField] private GameObject malePlayer;

    [SerializeField] private GameObject femalePlayer;

    [SerializeField] private GameObject ui;

    [SerializeField] private GameObject inventoryContainer;

    [SerializeField] private GameObject inGameUI;

    [SerializeField] private PlayerData[] playerDatas;

    [SerializeField] private GameObject[] allLevelObjects;

    [SerializeField] private GameObject survivalMap;

    private CameraFollow cameraFollow;

    private CameraConfiner cameraConfiner;

    public event Action OnStageClearedEvent;

    public event Action OnCampaignClearedEvent;

    private int currentCharacterIndex;

    private InventoryObject playerInventory;

    private bool isNewGame = false;

    public event Action OnSaveGameEvent;

    private bool isClearedCampaign;

    public event Action OnShowUnlockSurvivalTextEvent;

    public event Action<bool> OnUnlockSurvivalModeEvent;

    public event Action<bool> OnUnlockContinueGameButtonEvent;

    public bool isPauseGame = false;

    private bool hasSavedFile = false;

    private bool isOnSurvivalMode = false;

    private bool isPlayingInGameMusic = false;

    public event Action OnPlayInGameMusicEvent;

    private SoundManager soundManager;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        cameraFollow = FindObjectOfType<CameraFollow>();

        cameraConfiner = FindObjectOfType<CameraConfiner>();

        totalLevels = 3;

        currentLevel = 1;

        totalEnemies = 0;

        currentLevelObject = campaignMode.transform.Find("Level " + currentLevel);

        battleSystem = FindObjectOfType<BattleSystem>();

        soundManager = FindObjectOfType<SoundManager>();

        gameState = GameState.NOT_PLAYING;
    }

    private void Start()
    {
        HandleScenes.Instance.OnGameStartEvent += Instance_OnGameStartEvent;

        HandleScenes.Instance.OnGameEndEvent += Instance_OnGameEndEvent;

        HandleScenes.Instance.OnNewGameEvent += Instance_OnNewGameEvent;

        HandleScenes.Instance.OnContinueGameEvent += Instance_OnContinueGameEvent;

        HandleScenes.Instance.OnSurvivalModeStartEvent += Instance_OnSurvivalModeStartEvent;

        if (playerInventory == null)
        {
            isClearedCampaign = false;
        }
        else
        {
            isClearedCampaign = InventoryObject.Instance.Container.info.isClearedCampaign;
        }

        isClearedCampaign = true;

        OnUnlockSurvivalModeEvent?.Invoke(isClearedCampaign);

        if (playerInventory == null)
        {
            hasSavedFile = false;
        }
        else
        {
            hasSavedFile = true;
        }

        OnUnlockContinueGameButtonEvent?.Invoke(hasSavedFile);
    }


    private void OnEnable()
    {
        playerInventory = inventory.LoadPlayer();

        
    }

    public bool HasSavedFile()
    {
        return hasSavedFile;
    }

    private void Instance_OnNewGameEvent()
    {
        isNewGame = true;

        currentLevel = 1;
    }

    private void Instance_OnContinueGameEvent()
    {
        isNewGame = false;

        InventoryObject.Instance.LoadPlayer();

        currentLevel = InventoryObject.Instance.Container.info.currentLevel;

        currentCharacterIndex = InventoryObject.Instance.Container.info.characterIndex;
    }

    private void Instance_OnGameStartEvent()
    {
        Transform clone = allLevelObjects[currentLevel - 1].transform;

        clone.gameObject.SetActive(false);

        currentLevelObject = Instantiate(clone, campaignMode.transform);

        GetTotalEnemies();

        battleSystem.GetBattleTrigger();

        ManageLightForEachLevel();

        cameraConfiner.SetConfiner();

        SaveGame();

        OnSaveGameEvent?.Invoke();

        isPauseGame = false;

        gameState = GameState.IS_PLAYING;

        isPlayingInGameMusic = false;

        //inGameUI.SetActive(true);

        //StartCoroutine(InGameTransition.Instance.StartTransition());
    }

    private void Instance_OnGameEndEvent()
    {
        isNewGame = false;

        hasSavedFile = true;

        OnUnlockContinueGameButtonEvent?.Invoke(hasSavedFile);

        currentLevel++;

        GameObject existPlayer = GameObject.FindGameObjectWithTag("Player");

        Destroy(existPlayer);       

        for (int i = 0; i < inventoryContainer.transform.childCount; i++)
        {
            if(i == 0)
            {
                continue;
            }

            Destroy(inventoryContainer.transform.GetChild(i).gameObject);
        }

        inGameUI.SetActive(false);

        //currentLevelObject.gameObject.SetActive(false);

        gameState = GameState.NOT_PLAYING;

        Destroy(currentLevelObject.gameObject, 1f);

        if (currentLevel > totalLevels)
        {
            currentLevel = totalLevels;          

            if(isClearedCampaign == false)
            {
                isClearedCampaign = true;

                OnShowUnlockSurvivalTextEvent?.Invoke();

                OnUnlockSurvivalModeEvent?.Invoke(isClearedCampaign);
            }

            OnCampaignClearedEvent?.Invoke();
        }

        SaveGame();

        soundManager.StopBGMSound();

        SoundManager.Instance.StopSFXSound(SoundManager.SFXSound.Movement);
    }

    public Transform GetSurvivalMap()
    {
        return survivalMap.transform;
    }

    private void Instance_OnSurvivalModeStartEvent()
    {
        LightSystem.Instance.SetLight(1f, Color.white);

        gameState = GameState.IS_PLAYING;

        InventoryObject.Instance.Container.weapons = new List<WeaponAmmoAmount>();

        InventoryObject.Instance.Container.power = new List<PowerItemAmount>();
    }

    public void TurnOnSurvival()
    {
        isOnSurvivalMode = true;

        GameObject clone = Instantiate(survivalMap, campaignMode.transform);

        currentLevelObject = clone.transform;

        clone.SetActive(true);

        SpawnCharacter();

        cameraConfiner.SetConfiner();

        //survivalMap.SetActive(true);

        inGameUI.SetActive(true);
    }

    public void TurnOnLevel()
    {
        SpawnCharacter();
     
        currentLevelObject.gameObject.SetActive(true);

        inGameUI.SetActive(true);
    }

    public bool IsFinalLevel()
    {
        return currentLevel == totalLevels;
    }

    public InventoryObject GetPlayerInventory()
    {
        return playerInventory;
    }

    public int GetTotalEnemies()
    {
        totalEnemies = 0;

        foreach (var wave in currentLevelObject.GetComponentsInChildren<EnemySpawner>())
        {
            totalEnemies += wave.GetWaveEnemies();
        }

        return totalEnemies;
    }

    public void DecreaseEnemies(int value)
    {
        if (!isOnSurvivalMode)
        {
            totalEnemies -= value;

            if (!isPlayingInGameMusic) 
            {
                OnPlayInGameMusicEvent?.Invoke();

                isPlayingInGameMusic = true;
            } 

            if (totalEnemies == 1)
            {
                print("Only 1 left");

                Transform finalEnemy = currentLevelObject.GetComponentInChildren<EnemyMovement>().transform;

                OnArrowEvent?.Invoke(finalEnemy);
            }

            LevelCleared();
        }
    }

    void LevelCleared()
    {
        if (totalEnemies <= 0)
        {
            OnStageClearedEvent?.Invoke();
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }   

    public Transform GetCurrentLevelObject()
    {
        return currentLevelObject;
    }

    public bool IsSurvivalMode()
    {
        return isOnSurvivalMode;
    }

    public void SpawnCharacter()
    {
        GameObject playerClone = Instantiate(currentCharacterIndex == 0 ? malePlayer : femalePlayer, currentLevelObject.Find("Spawn Character").transform.position, Quaternion.identity, transform); ;

        playerClone.SetActive(true);

        cameraFollow.gameObject.SetActive(true);

        cameraFollow.TurnOnCameraFollow();
    }

    public void PlayerDead()
    {
        //var allEnemies = currentLevelObject.GetComponentsInChildren<EnemyMovement>();

        //foreach (var e in allEnemies)
        //{
        //    e.enabled = false;

        //    e.gameObject.GetComponent<EnemyDamage>().enabled = false;
        //}

        LightSystem.Instance.SetLight(1f, Color.red);

        HandleScenes.Instance.PlayerDeadBackToMenu();   
    }

    public void DisplaySurvivalResult()
    {
        inGameUI.SetActive(false);

        if (transform.childCount > 0)
        {
            GameObject existPlayer = transform.GetChild(0).gameObject;

            if (existPlayer)
            {
                Destroy(existPlayer);
            }
        }

        EnemyCounter.Instance.SetCanPlayInGameMusic(false);

        SoundManager.Instance.StopSFXSound(SoundManager.SFXSound.Movement);
    }

    public void BackToMainMenu()
    {
        isOnSurvivalMode = false;

        isPauseGame = false;

        gameState = GameState.NOT_PLAYING;

        if(transform.childCount > 0)
        {
            GameObject existPlayer = transform.GetChild(0).gameObject;

            if (existPlayer)
            {
                Destroy(existPlayer);
            }
        }              

        if(currentLevelObject != null)
        {
            currentLevelObject.gameObject.SetActive(false);

            Destroy(currentLevelObject?.gameObject);
        }

        for (int i = 0; i < inventoryContainer.transform.childCount; i++)
        {
            if (i == 0)
            {
                continue;
            }

            Destroy(inventoryContainer.transform.GetChild(i).gameObject);
        }

        inGameUI.SetActive(false);

        EnemyCounter.Instance.SetCanPlayInGameMusic(false);

        //soundManager.StopSound();

        isPlayingInGameMusic = false;
    }

    public RandomSpawnRate[] ChangeItemsOverTime()
    {
        List<RandomSpawnRate> newList = new List<RandomSpawnRate>();

        newList
                .AddRange(phase
                .Where(p => p.level == currentLevel)
                .FirstOrDefault().rates
                .ToList());

        return newList.ToArray();
    }


    private void ManageLightForEachLevel()
    {
        ChangeItemsOverTime();

        switch (currentLevel)
        {
            case 1:
                LightSystem.Instance.SetLight(0.3f, Color.white);

                break;
            case 2:
                LightSystem.Instance.SetLight(0.3f, Color.white);


                battleSystem.GetWaves().gameObject.SetActive(false);

                break;
            case 3:
                LightSystem.Instance.SetLight(1f, Color.white);

                battleSystem.GetWaves().gameObject.SetActive(false);

                break;
            default:

                break;
        }
    }

    public PlayerData[] GetPlayerDatas()
    {
        return playerDatas;
    }

    public void SetCharacterIndex(int index)
    {
        currentCharacterIndex = index;
    }

    public GameObject GetCurrentCharacterPref()
    {
        if(currentCharacterIndex == 0)
        {
            return malePlayer;
        }

        return femalePlayer;
    }

    public int GetCurrentCharacterIndex()
    {
        return currentCharacterIndex;
    }

    public List<WeaponItem> GetAllWeaponItems()
    {
        return database.GetWeapon.Values.ToList();
    }

    public List<PowerItem> GetAllPowerItems()
    {
        return database.GetPower.Values.ToList();
    }

    public bool IsNewGame()
    {
        return isNewGame;
    }

    public bool GetIsClearedCampaign()
    {
        return isClearedCampaign;
    }

    private void SaveGame()
    {
        //inventory.Container.weapons = ;

        //inventory.Container.info.money = GetCurrentCharacterPref().GetComponent<PlayerStats>().GetPlayerMoney();

        //inventory.Container.power = playerStats.GetPlayerPowers();

        inventory.Container.info.currentLevel = GetCurrentLevel();

        inventory.Container.info.characterIndex = GetCurrentCharacterIndex();

        inventory.Container.info.isClearedCampaign = isClearedCampaign;

        inventory.SavePlayer(inventory);
    }

    public float CalculateNumberToPercentage(float current, float total)
    {
        return Mathf.Round(current / (total / 100));
    }

    public float CalculatePercentageToNumber(float current, float total)
    {
        return Mathf.Round((total / 100) * current);
    }
}
