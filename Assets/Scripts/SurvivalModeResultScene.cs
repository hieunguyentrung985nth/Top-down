using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivalModeResultScene : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private TextMeshProUGUI resultTextUI;

    [SerializeField] private TextMeshProUGUI rankTextUI;

    [SerializeField] private TextMeshProUGUI dialogueTextUI;

    private SurvivalModeBattle survivalModeBattle;

    private bool textAnimation = false;

    private int minutes;

    private int seconds;

    private float currentTimer;

    private string resultText;

    private System.Random rand = new System.Random();

    private string rankText;

    private string dialogueText;

    private bool canSkip = false;

    private bool skipAnimation = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        survivalModeBattle = FindObjectOfType<SurvivalModeBattle>();       
    }

    private void Update()
    {
        if (skipAnimation)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StopAllCoroutines();

                StartCoroutine(ShowResult());              
            }
        }
        if (canSkip && !skipAnimation)
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameManager.BackToMainMenu();

                canSkip = false;

                HandleScenes.Instance.DisplayMainMenu(false, false);
            }
        }
    }

    private void OnEnable()
    {
        canSkip = false;

        skipAnimation = true;

        rankTextUI.text = "";

        dialogueTextUI.text = "";

        currentTimer = survivalModeBattle.GetTimer();

        StartCoroutine(GetResult());
    }

    private IEnumerator GetResult()
    {
        gameManager.GetCurrentLevelObject().gameObject.SetActive(false);

        textAnimation = true;

        StartCoroutine(PlayTextAnimation());

        yield return new WaitForSeconds(2f);

        textAnimation = false;        

        RankCalculate();

        resultText = "SURVIVED FOR: " + (minutes < 10 ? "0" + minutes : minutes) + ":" + (seconds < 10 ? "0" + seconds : seconds);

        resultTextUI.text = resultText;

        yield return StartCoroutine(DisplayRank());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(PlayDialogueText());

        canSkip = true;

        skipAnimation = false;
    }

    private IEnumerator ShowResult()
    {
        RankCalculate();

        resultText = "SURVIVED FOR: " + (minutes < 10 ? "0" + minutes : minutes) + ":" + (seconds < 10 ? "0" + seconds : seconds);

        resultTextUI.text = resultText;

        rankTextUI.text = rankText;

        rankTextUI.fontSize = 90;

        dialogueTextUI.text = dialogueText;

        skipAnimation = false;

        yield return new WaitForSecondsRealtime(1f);

        canSkip = true;
    }

    private IEnumerator DisplayRank()
    {
        rankTextUI.text = rankText;

        rankTextUI.fontSize = 2100;

        while(rankTextUI.fontSize != 90)
        {
            rankTextUI.fontSize -= 10f;

            yield return new WaitForSeconds(0.0001f);
        }
    }

    private IEnumerator PlayDialogueText()
    {
        for (int i = 0; i < dialogueText.Length; i++)
        {
            dialogueTextUI.text += dialogueText[i];

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void RankCalculate()
    {
        minutes = (int)currentTimer / 60;

        seconds = (int)currentTimer - minutes * 60;

        if (currentTimer <= 60)
        {
            rankText = "F";

            dialogueText = "You call yourself a gamer? Pfff.";
        }
        else if(currentTimer > 60 && currentTimer <= 120)
        {
            rankText = "E";

            dialogueText = "Keep fighting. Never give up.";
        }
        else if (currentTimer > 120 && currentTimer <= 180)
        {
            rankText = "D";

            dialogueText = "Not bad. Not bad at all.";
        }
        else if (currentTimer > 180 && currentTimer <= 240)
        {
            rankText = "C";

            dialogueText = "Oh? Looks like I've underestimated you.";
        }
        else if (currentTimer > 240 && currentTimer <= 300)
        {
            rankText = "B";

            dialogueText = "Almost there. Keep going!.";
        }
        else if (currentTimer > 300 && currentTimer <= 360)
        {
            rankText = "A";

            dialogueText = "You are really good at this game.";
        }
        else if (currentTimer > 360)
        {
            rankText = "S";

            dialogueText = "You are a GOD!.";
        }
    }

    private IEnumerator PlayTextAnimation()
    {      
        while (textAnimation)
        {
            minutes = (int)Mathf.Ceil((float)rand.NextDouble() * 100);

            seconds = (int)Mathf.Ceil((float)rand.NextDouble() * 100);

            resultText = $"RESULT: {minutes}:{seconds}";

            resultTextUI.text = resultText;

            yield return new WaitForSeconds(0.01f);
        }
    }
}
