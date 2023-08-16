using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TransitionAnimation : MonoBehaviour
{
    public static TransitionAnimation Instance { get; private set; }

    private Animator anim;

    private bool isTransitioning;

    private GameObject currentSceneOff;

    private GameObject currentSceneOn;

    private bool isGameStart = false;

    [SerializeField] private GameObject inGameUI;

    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject clearGameScene;

    private GameManager gameManager;

    private bool canSkip = false;

    public event Action OnUnlockSurvivalModeTextEvent;

    private SoundManager soundManager;

    private void Awake()
    {
        Instance = this;

        anim = GetComponent<Animator>();

        gameManager = FindObjectOfType<GameManager>();

        soundManager = FindObjectOfType<SoundManager>();

        isTransitioning = false;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isTransitioning && isGameStart == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EndTransition(currentSceneOn, currentSceneOff);
            }
        }

        if (Input.GetMouseButtonDown(0) && canSkip)
        {
            canSkip = false;

            StartCoroutine(StartTransition3(mainMenu, clearGameScene));
        }

    }

    public IEnumerator StartTransition(GameObject sceneOff = null, GameObject sceneOn = null, bool isGameStart = false, bool isGameEnd = false, bool isOpenScene = false, bool isPauseGame = false, bool isSurvival = false, bool canSkip = true, bool playMusic = false)
    {
        gameObject.SetActive(true);

        this.isGameStart = isGameStart;

        if (isGameEnd || isOpenScene || isPauseGame || isSurvival || !canSkip)
        {
            isTransitioning = false;
        }
        else
        {
            isTransitioning = true;
        }

        if (isPauseGame)
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        currentSceneOff = sceneOff;

        currentSceneOn = sceneOn;

        anim.speed = 1f;

        anim.ResetTrigger("startTransition");

        if (isGameEnd)
        {
            anim.speed = 1.25f;
        }

        else if (isOpenScene)
        {
            anim.speed = 0.5f;
        }

        anim.SetTrigger("startTransition");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        sceneOff?.SetActive(false);

        if (isGameStart)
        {
            //yield return InGameTransition.Instance.StartTransition(gameObject);

            //yield return null;

            anim.SetTrigger("endTransition");

            anim.speed = 1.25f;

            gameManager.TurnOnLevel();

            HandleScenes.Instance.TurnOnCrosshair();

            SoundManager.Instance.StopBGMSound();

            yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

            anim.speed = 1f;

            gameObject.SetActive(false);

            isTransitioning = false;
        }

        else if (isGameEnd)
        {
            inGameUI.SetActive(false);          

            anim.speed = 1f;

            anim.SetTrigger("endTransition");

            if (!gameManager.IsSurvivalMode())
            {
                gameManager.BackToMainMenu();
            }
            else
            {
                gameManager.DisplaySurvivalResult();
            }

            sceneOn?.SetActive(true);

            SoundManager.Instance.StopBGMSound();

            SoundManager.Instance.PlayBGMSound(SoundManager.BGMSound.MainMenuTheme);

            yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

            gameObject.SetActive(false);

            isTransitioning = false;
        }

        else if (isSurvival)
        {
            anim.SetTrigger("endTransition");

            anim.speed = 1.25f;

            gameManager.TurnOnSurvival();

            HandleScenes.Instance.TurnOnCrosshair();

            SoundManager.Instance.StopBGMSound();

            yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

            anim.speed = 1f;

            gameObject.SetActive(false);

            isTransitioning = false;
        }

        anim.SetTrigger("endTransition");

        if (isOpenScene)
        {
            anim.speed = 0.5f;
        }
        else
        {
            anim.speed = 1f;
        }

        sceneOn?.SetActive(true);

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.updateMode = AnimatorUpdateMode.Normal;

        if (playMusic)
        {
            SoundManager.Instance.StopBGMSound();

            SoundManager.Instance.PlayBGMSound(SoundManager.BGMSound.MainMenuTheme);
        }

        if (isOpenScene)
        {
            gameObject.SetActive(true);

            isTransitioning = false;

            //soundManager.PlayBGMSound(SoundManager.BGMSound.MainMenuTheme);
        }
        else if (isPauseGame)
        {
            gameObject.SetActive(false);

            isTransitioning = false;
        }
        else
        {
            gameObject.SetActive(false);

            isTransitioning = true;
        }
    }

    public IEnumerator StartTransition2(GameObject sceneOn, bool isClearedCampaign = false, bool isUnlockSurvivalMode = false)
    {
        gameObject.SetActive(true);

        anim.ResetTrigger("startTransition");

        anim.SetTrigger("startTransition");

        anim.speed = 1.25f;      

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        sceneOn.SetActive(true);

        if (isUnlockSurvivalMode)
        {
            OnUnlockSurvivalModeTextEvent?.Invoke();
        }

        anim.SetTrigger("endTransition");

        anim.speed = 1f;

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.ResetTrigger("endTransition");

        soundManager.PlayBGMSound(SoundManager.BGMSound.MainMenuTheme);

        //gameManager.GetCurrentLevelObject().gameObject.SetActive(false);

        if (isClearedCampaign)
        {
            canSkip = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
        

        //Invoke(nameof(TurnOff), 0.9f);

        
    }

    public IEnumerator StartTransition3(GameObject sceneOn, GameObject sceneOff)
    {
        gameObject.SetActive(true);

        anim.ResetTrigger("startTransition");

        anim.ResetTrigger("endTransition");

        anim.SetTrigger("startTransition");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        sceneOff.SetActive(false);

        anim.ResetTrigger("endTransition");

        anim.SetTrigger("endTransition");

        anim.speed = 1f;

        sceneOn.SetActive(true);

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);       

        gameObject.SetActive(false);


        //Invoke(nameof(TurnOff), 0.9f);


    }

    private void EndTransition(GameObject sceneOn, GameObject sceneOff = null)
    {
        HandleScenes.Instance.StopAllCoroutines();

        isTransitioning = false;

        anim.ResetTrigger("startTransition");

        anim.ResetTrigger("endTransition");

        sceneOff?.SetActive(false);

        sceneOn?.SetActive(true);

        currentSceneOff = sceneOff;

        currentSceneOn = sceneOn;

        gameObject.SetActive(false);
    }
}
