using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClearGameScene : MonoBehaviour
{
    private TextMeshProUGUI sceneTextThanks;

    private TextMeshProUGUI unlockSurvival;

    private GameManager gameManager;

    [SerializeField, TextArea(3,5)] private string textThanksForPlaying;

    [SerializeField, TextArea(3, 5)] private string textWithUnlockSurvival;

    [SerializeField] private GameObject mainMenu;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        sceneTextThanks = transform.Find("Clear Game Text").GetComponent<TextMeshProUGUI>();

        unlockSurvival = transform.Find("Unlock Survival Mode").GetComponent<TextMeshProUGUI>();

        sceneTextThanks.text = textThanksForPlaying;

        unlockSurvival.text = textWithUnlockSurvival;

        TransitionAnimation.Instance.OnUnlockSurvivalModeTextEvent += Instance_OnUnlockSurvivalModeTextEvent;
    }

    private void Instance_OnUnlockSurvivalModeTextEvent()
    {
        unlockSurvival.gameObject.SetActive(true);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {       
        unlockSurvival.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //gameManager.OnShowUnlockSurvivalTextEvent -= GameManager_OnShowUnlockSurvivalTextEvent;
    }

}
