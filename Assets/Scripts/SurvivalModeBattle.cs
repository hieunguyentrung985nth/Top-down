using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static EnemySpawner;
using static GameManager;

public class SurvivalModeBattle : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnRate
    {
        public GameObject prefab;

        [Range(0, 100f)]
        public float currentRate;

        [Range(0, 100f)]
        public float minRate;

        [Range(0, 100f)]
        public float maxRate;
    }

    [Serializable]
    public class RandomSpawnEnemyWave
    {
        public EnemySpawnRate[] randomSpawnRates;

        public GameObject waveBoss;

        public float timer;

        public float timeToWait;

        public float timeBossBattle;
    }

    [Serializable]
    public class RandomEnemy
    {
        public RandomSpawnEnemyWave[] randomSpawnEnemyWaves;

        public Transform[] spawnLocations;
    }

    [Serializable]
    public class MultiplyTimer
    {
        public int waveIndex;

        public float minTimer;

        public float maxTimer;
    }

    [SerializeField] private RandomEnemy wave;

    [SerializeField] private Transform enemiesHolder;

    [SerializeField] private MultiplyTimer[] multiplyList;

    [SerializeField] private PhaseSpawnRate[] phase;

    private System.Random rand = new System.Random();

    private int waveIndex = 0;

    private float totalWeights = 0;

    private float currentTimer = 0;

    private float timerCounter = 0;

    private MultiplyTimer currentMultiply;

    private float multiply;

    private bool continueSpawn;

    private GameManager gameManager;

    private RandomSpawnEnemyWave currentSpawnRate;

    public event Action<Transform> OnBossAppearEvent;

    private bool isPlayerAlive = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        currentSpawnRate = wave.randomSpawnEnemyWaves[waveIndex];

        SetUpCurrentRate();

        CalculateWeights();
    }

    private void Start()
    {
        EnemyCounter.Instance.ResetTotalEnemies();

        StartCoroutine(Timer());

        StartCoroutine(SpawnRandomEnemy());
    }

    private void Update()
    {
        // if no more enemies and last wave then end mode
        if (EnemyCounter.Instance.GetTotalEnemies() <= 0 && waveIndex > wave.randomSpawnEnemyWaves.Length - 1)
        {
            //OnSurvivalModeEndEvent?.Invoke();

            Instance_OnSurvivalModeVictoryEvent();
        }
    }

    private void OnEnable()
    {
        //EnemyCounter.Instance.OnSurvivalModeEndEvent += Instance_OnSurvivalModeVictoryEvent;
    }

    private void OnDisable()
    {
        //EnemyCounter.Instance.OnSurvivalModeEndEvent -= Instance_OnSurvivalModeVictoryEvent;
    }

    private void Instance_OnSurvivalModeVictoryEvent()
    {
        StopCoroutine(nameof(Timer));

        isPlayerAlive = false;

        DisplayResultScene();
    }

    private void SetUpCurrentRate()
    {
        // set current rate for current wave
        for (int i = 0; i < currentSpawnRate.randomSpawnRates.Length; i++)
        {
            currentSpawnRate.randomSpawnRates[i].currentRate = currentSpawnRate.randomSpawnRates[i].minRate;
        }
    }

    private int GetRandomSpawnLocationIndex()
    {
        int r = rand.Next(0, 17);

        for (int i = 0; i < wave.spawnLocations.Length; i++)
        {
            if (i == r)
            {
                return i;
            }
        }

        return 0;
    }

    private void CalculateWeights()
    {
        float total = 0;

        for (int i = 0; i < currentSpawnRate.randomSpawnRates.Length; i++)
        {
            total += currentSpawnRate.randomSpawnRates[i].currentRate;
        }

        totalWeights = total;
    }

    private int GetRandomEnemyIndex()
    {
        float r = (float)rand.NextDouble();

        float adding = 0f;

        for (int i = 0; i < currentSpawnRate.randomSpawnRates.Length; i++)
        {
            if (currentSpawnRate.randomSpawnRates[i].currentRate / totalWeights + adding >= r)
            {
                return i;
            }
            else
            {
                adding += currentSpawnRate.randomSpawnRates[i].currentRate / totalWeights;
            }
        }

        return -1;
    }

    private float GetRandomTimeToSpawn()
    {
        float r;

        // change multiphy over time
        multiply = Mathf.Lerp(currentMultiply.minTimer, currentMultiply.maxTimer, timerCounter);

        r = (float)rand.NextDouble() * multiply;

        return r;
    }

    private IEnumerator SpawnRandomEnemy()
    {
        // if can spawn and player alive
        while(continueSpawn && isPlayerAlive)
        {
            // get random enemy
            EnemySpawnRate e = currentSpawnRate.randomSpawnRates[GetRandomEnemyIndex()];

            GameObject enemyObject = e.prefab;

            Transform randomSpawnLocation = wave.spawnLocations[GetRandomSpawnLocationIndex()];

            Instantiate(enemyObject, randomSpawnLocation.position, Quaternion.identity, enemiesHolder);

            EnemyCounter.Instance.IncreaseEnemy();

            yield return new WaitForSeconds(GetRandomTimeToSpawn());         
        }       
    }

    private IEnumerator Timer()
    {
        currentMultiply = multiplyList[GetMultiplyTimer()];

        continueSpawn = true;

        isPlayerAlive = true;

        // if player still alive
        while (isPlayerAlive)
        {
            currentTimer += 1f;

            // if can spawn then change enemy over time
            if (continueSpawn)
            {
                timerCounter += 1f;

                if(waveIndex > 0)
                {
                    ChangeEnemyRate();
                }               
            }          

            // if done this wave
            if (timerCounter >= currentSpawnRate.timer && continueSpawn)
            {
                // stop spawn
                continueSpawn = false;

                // reset timer
                timerCounter -= currentSpawnRate.timer;

                // then spawn Boss
                StartCoroutine(SpawnWaveBoss());
            }

            print(currentTimer);

            yield return new WaitForSeconds(1f);
        }        
    }

    private int GetMultiplyTimer()
    {
        for (int i = 0; i < multiplyList.Length; i++)
        {
            if(waveIndex == multiplyList[i].waveIndex)
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator SpawnWaveBoss()
    {
        // wait 
        yield return new WaitForSeconds(currentSpawnRate.timeToWait);

        GameObject enemyObject = currentSpawnRate.waveBoss;

        // if current wave has boss
        if(enemyObject != null)
        {
            Transform randomSpawnLocation = wave.spawnLocations[GetRandomSpawnLocationIndex()];

            GameObject boss = Instantiate(enemyObject, randomSpawnLocation.position, Quaternion.identity, enemiesHolder);

            OnBossAppearEvent?.Invoke(boss.transform);

            EnemyCounter.Instance.IncreaseEnemy();

            yield return new WaitForSeconds(currentSpawnRate.timeBossBattle);
        }

        // next wave
        waveIndex++;

        // if final wave then no more spawning
        if (waveIndex > wave.randomSpawnEnemyWaves.Length - 1)
        {
            continueSpawn = false;

            print("No more wave");
        }
        else
        {
            continueSpawn = true;

            StartCoroutine(SpawnRandomEnemy());

            GetNewSpawnRate();

            CalculateWeights();

            print("Next Wave");
        }       
    }

    private void GetNewSpawnRate()
    {
        // get new multiply then reset rate
        currentMultiply = multiplyList[GetMultiplyTimer()];

        currentSpawnRate = wave.randomSpawnEnemyWaves[waveIndex];

        SetUpCurrentRate();
    }

    private void ChangeEnemyRate()
    {
        float interpolatedValue = gameManager.CalculateNumberToPercentage(timerCounter, currentSpawnRate.timer) / 100f;

        for (int i = 0; i < currentSpawnRate.randomSpawnRates.Length; i++)
        {
            currentSpawnRate.randomSpawnRates[i].currentRate = Mathf.Lerp(currentSpawnRate.randomSpawnRates[i].minRate, currentSpawnRate.randomSpawnRates[i].maxRate, interpolatedValue);
        }

        CalculateWeights();       
    }

    public RandomSpawnRate[] ChangeItemsOverTime()
    {
        List<RandomSpawnRate> newList = new List<RandomSpawnRate>();

        if(waveIndex < wave.randomSpawnEnemyWaves.Length - 1)
        {
            newList
                .AddRange(phase
                .Where(p => p.level == waveIndex)
                .FirstOrDefault().rates
                .ToList());
        }
        else
        {
            return null;
        }

        return newList.ToArray();
    }

    private void DisplayResultScene()
    {
        HandleScenes.Instance.SurvivalModeEnd();
    }

    public float GetTimer()
    {
        return currentTimer;
    }
}
