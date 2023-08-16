using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    private Transform arrow;

    private GameManager gameManager;

    private SurvivalModeBattle survivalModeBattle;

    private List<Transform> enemyToPoint = new List<Transform>();

    private int currentEnemyPointIndex = 0;

    private void Awake()
    {
        arrow = transform.Find("Arrow");

        gameManager = FindObjectOfType<GameManager>();

        survivalModeBattle = FindObjectOfType<SurvivalModeBattle>();
    }

    private void Update()
    {
        CheckIfEnemyDie();

        RotateArrow();
    }

    private void OnEnable()
    {
        gameManager.OnArrowEvent += GameManager_OnArrowEvent;

        if(survivalModeBattle != null)
        {
            survivalModeBattle.OnBossAppearEvent += SurvivalModeBattle_OnBossAppearEvent;
        }       
    }


    private void OnDisable()
    {
        gameManager.OnArrowEvent -= GameManager_OnArrowEvent;

        if (survivalModeBattle != null)
        {
            survivalModeBattle.OnBossAppearEvent -= SurvivalModeBattle_OnBossAppearEvent;
        }
    }

    private void SurvivalModeBattle_OnBossAppearEvent(Transform enemy)
    {
        DetectEnemy(enemy);
    }

    private void GameManager_OnArrowEvent(Transform enemy)
    {
        DetectEnemy(enemy);
    }

    private void DisplayArrow()
    {
        arrow.GetChild(0).gameObject.SetActive(true);
    }

    private void HideArrow()
    {
        arrow.GetChild(0).gameObject.SetActive(false);
    }

    private void DetectEnemy(Transform enemy)
    {
        enemyToPoint.Add(enemy);

        if (enemy != null)
        {
            DisplayArrow();

            RotateArrow();
        }       
    }

    private void RotateArrow()
    {
        if(enemyToPoint.Count > 0 && enemyToPoint[currentEnemyPointIndex] != null)
        {
            Vector2 direction = (enemyToPoint[currentEnemyPointIndex].transform.position - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            arrow.transform.eulerAngles = new Vector3(0, 0, angle + 90);
        }
        else
        {
            HideArrow();
        }
    }

    private void CheckIfEnemyDie()
    {
        if(enemyToPoint.Count > 0)
        {
            if (enemyToPoint[currentEnemyPointIndex] == null)
            {
                enemyToPoint.RemoveAt(currentEnemyPointIndex);

                currentEnemyPointIndex = 0;
            }
        }       
    }
}
