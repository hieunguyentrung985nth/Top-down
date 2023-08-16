using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandleScenes : MonoBehaviour
{
    public static HandleScenes Instance { get; private set; }

    [Header("Menu Container")]
    [SerializeField] private GameObject menuContainer;

    [SerializeField] private GameObject clearCampaignScene;


    [Header("Character Selection")]
    [SerializeField] private GameObject characterSelectionMenu;

    [SerializeField] private Button[] characterButtons;

    [SerializeField] private Button startCampaignMode;

    [SerializeField] private Button startSurvivalMode;

    [SerializeField] private Button backToMenu;


    [Header("Shop Menu")]
    [SerializeField] private GameObject shopMenu;

    [SerializeField] private Button continueButtonStartCampain;

    [SerializeField] private Button backButtonShopMenu;

    [SerializeField] private Button hackButtonShopMenu;

    [SerializeField] private TextMeshProUGUI hackTextShopMenu;


    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Button newGame;

    [SerializeField] private Button continueGame;

    [SerializeField] private Button exitGame;

    [Header("Pause Game Menu")]
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private Button pauseMenuBackButton;

    [SerializeField] private Button pauseMenuBackMenuButton;

    [Header("Option Menu")]
    [SerializeField] private GameObject optionMenu;

    [SerializeField] private Button optionMenuButton;

    [SerializeField] private Button backOptionMenuButton;

    [Header("Block")]
    [SerializeField] private GameObject blockRaycast;

    [Header("Open Scenes")]
    [SerializeField] private GameObject openScene;

    [Header("Survival Result")]
    [SerializeField] private GameObject survivalResultScene;

    [Header("Crosshair")]
    [SerializeField] private GameObject crosshair;

    private GameManager gameManager;

    public event Action OnNewGameEvent;

    public event Action OnContinueGameEvent;

    public event Action OnGameStartEvent;

    public event Action OnGameEndEvent;

    private bool isStageCleared;

    private GameObject currentMenuActive;

    private bool escPressed = false;

    private bool stopSpamming = false;

    private bool unlockSurvival = false;

    private bool isButotnSpamming = false;

    public event Action OnSurvivalModeStartEvent;

    private SoundManager soundManager;

    private void Awake()
    {
        Instance = this;

        clearCampaignScene.SetActive(false);

        gameManager = FindObjectOfType<GameManager>();

        soundManager = FindObjectOfType<SoundManager>();

        gameManager.OnUnlockSurvivalModeEvent += GameManager_OnLockSurvivalModeEvent;

        gameManager.OnUnlockContinueGameButtonEvent += GameManager_OnUnlockContinueGameButtonEvent;

        currentMenuActive = mainMenu;

        newGame.onClick.AddListener(() =>
        {
            DisplayCharacterSelection();
        });

        backToMenu.onClick.AddListener(() =>
        {
            DisplayMainMenu(false);
        });

        continueButtonStartCampain.onClick.AddListener(() =>
        {
            blockRaycast.SetActive(true);

            StartCampain();
        });

        optionMenuButton.onClick.AddListener(() =>
        {
            DisplayOptionsMenu();
        });

        backOptionMenuButton.onClick.AddListener(() =>
        {
            DisplayMainMenu(false);
        });

        characterButtons[0].onClick.AddListener(() =>
        {
            CharacterSelection.Instance.ChooseCharacter(0);
        });

        characterButtons[1].onClick.AddListener(() =>
        {
            CharacterSelection.Instance.ChooseCharacter(1);
        });

        startCampaignMode.onClick.AddListener(() =>
        {
            DisplayShopMenu();

            DataBaseTemp.Instance.Initialize(gameManager.GetCurrentCharacterIndex());
        });

        backButtonShopMenu.onClick.AddListener(() =>
        {
            DisplayMainMenu(false);
        });

        pauseMenuBackButton.onClick.AddListener(() =>
        {
            StartCoroutine(BackToGame());
        });

        pauseMenuBackMenuButton.onClick.AddListener(() =>
        {
            gameManager.BackToMainMenu();

            TurnOffCrosshair();

            DisplayMainMenu(true, playMusic: true);

            Time.timeScale = 1;

            escPressed = false;

            stopSpamming = false;
        });

        hackButtonShopMenu.onClick.AddListener(() =>
        {
            StartCoroutine(HackMoney());
        });

        exitGame.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Start()
    {
        gameManager.OnStageClearedEvent += GameManager_OnStageClearedEvent;

        gameManager.OnCampaignClearedEvent += GameManager_OnCampaignClearedEvent;

        gameManager.OnShowUnlockSurvivalTextEvent += GameManager_OnShowUnlockSurvivalTextEvent;

        StartCoroutine(DisplayOpenScenes());
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isStageCleared && gameManager.gameState == GameManager.GameState.IS_PLAYING)
            {
                if (!gameManager.IsFinalLevel())
                {
                    EndCampain();
                }
                else
                {
                    EndFinalCampain();
                }

                TurnOffCrosshair();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!stopSpamming && gameManager.gameState == GameManager.GameState.IS_PLAYING)
            {
                if (!escPressed)
                {
                    StartCoroutine(PauseGame());
                }
                else
                {
                    StartCoroutine(BackToGame());
                }
            }
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        continueButtonStartCampain.onClick.RemoveListener(() => StartCampain());

        gameManager.OnStageClearedEvent -= GameManager_OnStageClearedEvent;

        gameManager.OnCampaignClearedEvent -= GameManager_OnCampaignClearedEvent;

        gameManager.OnShowUnlockSurvivalTextEvent -= GameManager_OnShowUnlockSurvivalTextEvent;
    }


    private void GameManager_OnUnlockContinueGameButtonEvent(bool obj)
    {
        CheckFileSaved(obj);
    }

    private void CheckFileSaved(bool value)
    {
        ColorBlock colors = continueGame.colors;

        continueGame.onClick.RemoveAllListeners();

        if (value)
        {            
            colors.normalColor = new Color32(0, 0, 0, 114);
            colors.highlightedColor = new Color32(135, 32, 92, 255);
            colors.pressedColor = new Color32(135, 32, 92, 0);
            colors.selectedColor = new Color32(135, 32, 92, 255);

            continueGame.onClick.AddListener(() =>
            {
                ContinueGame();
            });
        }
        else
        {
            colors.normalColor = new Color32(255, 255, 255, 65);
            colors.highlightedColor = new Color32(255, 255, 255, 65);
            colors.pressedColor = new Color32(255, 255, 255, 65);
            colors.selectedColor = new Color32(255, 255, 255, 65);

            continueGame.onClick.AddListener(() =>
            {
                StartCoroutine(LockContinueGameButton());
            });
        }

        continueGame.colors = colors;
    }

    private IEnumerator LockContinueGameButton()
    {
        if (isButotnSpamming == false)
        {
            isButotnSpamming = true;

            yield return StartCoroutine(PopupAnimation.Instance.StartPopUp("No Data Found"));

            isButotnSpamming = false;
        }
    }

    private void ContinueGame()
    {
        OnContinueGameEvent?.Invoke();

        DataBaseTemp.Instance.Initialize(gameManager.GetCurrentCharacterIndex());

        DisplayShopMenu();
    }


    private void GameManager_OnLockSurvivalModeEvent(bool obj)
    {       
        ColorBlock colors = startSurvivalMode.colors;

        startSurvivalMode.onClick.RemoveAllListeners();

        if (obj == true)
        {
            colors.normalColor = new Color32(0, 0, 0, 114);
            colors.highlightedColor = new Color32(135, 32, 92, 255);
            colors.pressedColor = new Color32(135, 32, 92, 0);
            colors.selectedColor = new Color32(135, 32, 92, 255);

            startSurvivalMode.onClick.AddListener(() =>
            {
                StartSurvivalMode();
            });
        }
        else
        {
            colors.normalColor = new Color32(255, 255, 255, 65);
            colors.highlightedColor = new Color32(255, 255, 255, 65);
            colors.pressedColor = new Color32(255, 255, 255, 65);
            colors.selectedColor = new Color32(255, 255, 255, 65);

            startSurvivalMode.onClick.AddListener(() =>
            {
                StartCoroutine(LockSurvivalModeButton());
            });
        }

        startSurvivalMode.colors = colors;
    }

    private void StartSurvivalMode()
    {
        DataBaseTemp.Instance.Initialize(gameManager.GetCurrentCharacterIndex());

        OnSurvivalModeStartEvent?.Invoke();

        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, isSurvival: true));
    }

    private IEnumerator LockSurvivalModeButton()
    {
        if(isButotnSpamming == false)
        {
            isButotnSpamming = true;

            yield return StartCoroutine(PopupAnimation.Instance.StartPopUp("Unlock after clearing Campaign Mode"));

            isButotnSpamming = false;
        }      
    }

    private IEnumerator PauseGame()
    {
        stopSpamming = true;

        gameManager.isPauseGame = true;

        Time.timeScale = 0;

        TurnOffCrosshair();

        yield return StartCoroutine(TransitionAnimation.Instance.StartTransition(sceneOn: pauseMenu, isPauseGame: true));

        escPressed = true;           

        currentMenuActive = pauseMenu;

        stopSpamming = false;
    }

    private IEnumerator BackToGame()
    {
        stopSpamming = true;

        yield return StartCoroutine(TransitionAnimation.Instance.StartTransition(pauseMenu, isPauseGame: true));

        TurnOnCrosshair();

        gameManager.isPauseGame = false;

        Time.timeScale = 1;

        escPressed = false;

        stopSpamming = false;
    }

    private IEnumerator DisplayOpenScenes()
    {
        GameObject open1 = openScene.transform.Find("Open 1").gameObject;

        GameObject open2 = openScene.transform.Find("Open 2").gameObject;

        GameObject blank = openScene.transform.Find("Blank Scene").gameObject;

        currentMenuActive = openScene;

        yield return new WaitForSecondsRealtime(2f);

        yield return StartCoroutine(TransitionAnimation.Instance.StartTransition(blank, sceneOn: open1, isOpenScene: true));

        //yield return new WaitForSecondsRealtime(0.3f);

        SoundManager.Instance.PlayBGMSound(SoundManager.BGMSound.MainMenuTheme);

        yield return StartCoroutine(TransitionAnimation.Instance.StartTransition(open1, open2, isOpenScene: true));

        DisplayMainMenu(true);

        //StartCoroutine(TransitionAnimation.Instance.StartTransition(openScene, mainMenu, isGameEnd: true));

        currentMenuActive = mainMenu;

    }

    private void DisplayShopMenu()
    {
        //StopAllCoroutines();

        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, shopMenu));

        currentMenuActive = shopMenu;

        print(InventoryObject.Instance.Container.info.money.ToString());
    }

    public event Action OnHackMoney;

    private IEnumerator HackMoney()
    {
        hackTextShopMenu.gameObject.SetActive(true);

        hackButtonShopMenu.enabled = false;

        InventoryObject.Instance.Container.info.money += 50000;

        OnHackMoney?.Invoke();

        float timer = 1f;

        while (timer > 0)
        {
            hackTextShopMenu.gameObject.transform.position += new Vector3(0, 2f, hackTextShopMenu.gameObject.transform.position.z);

            timer -= Time.deltaTime;

            print(timer);

            yield return null;
        }

        hackTextShopMenu.gameObject.SetActive(false);
    }

    private void DisplayCharacterSelection()
    {
        //StopAllCoroutines();

        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, characterSelectionMenu));

        currentMenuActive = characterSelectionMenu;

        OnNewGameEvent?.Invoke();
    }

    public void DisplayMainMenu(bool isPauseGame, bool canSkip = true, bool playMusic = false)
    {
        //StopAllCoroutines();

        gameManager.gameState = GameManager.GameState.NOT_PLAYING;

        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, mainMenu, isPauseGame: isPauseGame, canSkip: canSkip, playMusic: playMusic));

        currentMenuActive = mainMenu;
    }

    public void PlayerDeadBackToMenu()
    {
        gameManager.gameState = GameManager.GameState.NOT_PLAYING;

        TurnOffCrosshair();

        StartCoroutine(TransitionAnimation.Instance.StartTransition(sceneOn: mainMenu, isGameEnd: true));

        currentMenuActive = mainMenu;
    }

    public void SurvivalModeEnd()
    {
        gameManager.gameState = GameManager.GameState.NOT_PLAYING;

        TurnOffCrosshair();

        StartCoroutine(TransitionAnimation.Instance.StartTransition(sceneOn: survivalResultScene, isGameEnd: true));

        currentMenuActive = survivalResultScene;
    }

    public void TurnOffCrosshair()
    {
        crosshair.SetActive(false);
    }

    public void StartCampain()
    {
        //menuContainer.SetActive(false);

        //StopAllCoroutines();

        isStageCleared = false;

        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, isGameStart: true));

        OnGameStartEvent?.Invoke();
    }

    public void TurnOnCrosshair()
    {
        crosshair.SetActive(true);
    }

    public void EndCampain()
    {
        //menuContainer.SetActive(true);

        //StopAllCoroutines();

        OnGameEndEvent?.Invoke();

        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, shopMenu, isGameEnd: true));

        currentMenuActive = shopMenu;

        isStageCleared = false;

        TurnOffCrosshair();
    }

    private void DisplayOptionsMenu()
    {
        StartCoroutine(TransitionAnimation.Instance.StartTransition(currentMenuActive, optionMenu));

        currentMenuActive = optionMenu;
    }

    public void EndFinalCampain()
    {
        //menuContainer.SetActive(true);

        OnGameEndEvent?.Invoke();

        isStageCleared = false;

        currentMenuActive = mainMenu;
    }

    private void GameManager_OnStageClearedEvent()
    {
        isStageCleared = true;
    }

    private void GameManager_OnCampaignClearedEvent()
    {
        shopMenu.SetActive(false);

        //StopAllCoroutines();

        StartCoroutine(TransitionAnimation.Instance.StartTransition2(clearCampaignScene, isClearedCampaign: true, unlockSurvival));

        //clearCampaignScene.SetActive(true);
        unlockSurvival = false;
    }

    private void GameManager_OnShowUnlockSurvivalTextEvent()
    {
        unlockSurvival = true;
    }
}
