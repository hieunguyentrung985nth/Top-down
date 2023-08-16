using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private EnemyStats stats;

    public float currentHealth;

    public float currentGunHealth;

    //UIManager ui;

    private GameManager gameManager;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();

        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        currentGunHealth = stats.GetGunHealth();

        currentHealth = stats.GetHealth() ;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    private void TurnOnMovement()
    {
        GetComponent<EnemyMovement>().enabled = true;
    }

    public void TakeDamage(float receiveDamage)
    {
        if(currentGunHealth != 0 && stats.IsEnemyHasGun())
        {
            currentGunHealth = Mathf.Clamp(currentGunHealth - receiveDamage, 0, stats.GetGunHealth());

            if(currentGunHealth <= 0)
            {
                transform.Find("Guns").gameObject.SetActive(false);

                GetComponent<EnemyMovement>().enabled = false;

                Invoke(nameof(TurnOnMovement), 1f);

                GetComponent<EnemyDamage>().enabled = true;

                GetComponent<EnemyGun>().enabled = false;

                if (gameManager.IsSurvivalMode())
                {
                    GetComponent<Box>().SpawnFirstItem();
                }
            }
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth - receiveDamage, 0, stats.GetHealth());

            if (currentHealth <= 0)
            {
                if (!isSimulation)
                {
                    SoundManager.Instance.PlaySFXSound(SoundManager.Instance.GetEnemyDieSound(stats.GetEnemy()));
                }
                
                GetComponent<Box>().DisplayItem();

                Destroy(gameObject);
            }
        }
    }

    private bool isSimulation = false;

    public bool IsSimulation(bool value)
    {
        return isSimulation = value;
    }

    private void OnDestroy()
    {
        if (gameManager.IsSurvivalMode())
        {
            EnemyCounter.Instance.DecreaseEnemy();
        }
        else
        {
            gameManager.DecreaseEnemies(1);
        }
    }
}
