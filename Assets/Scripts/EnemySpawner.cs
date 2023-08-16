using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class RandomSpawnRate
    {
        public GameObject prefab;

        [Range(0, 100f)]
        public float rate;
    }

    [Serializable]
    public class RandomSpawnEnemyWave
    {
        public RandomSpawnRate[] randomSpawnRates;

        public Transform location;

        public bool status = false;

        public int count;
    }

    [SerializeField] private RandomSpawnEnemyWave wave;

    private float totalWeights;

    private System.Random rand = new System.Random();

    private List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        totalWeights = CalculateWeights();
    }

    private int GetRandomEnemyIndex()
    {
        float r = (float)rand.NextDouble();

        float adding = 0f;

        for (int i = 0; i < wave.randomSpawnRates.Length; i++)
        {
            if (wave.randomSpawnRates[i].rate / totalWeights + adding >= r)
            {
                return i;
            }
            else
            {
                adding += wave.randomSpawnRates[i].rate / totalWeights;
            }
        }
        return -1;
    }

    private float CalculateWeights()
    {
        float total = 0;

        for (int i = 0; i < wave.randomSpawnRates.Length; i++)
        {
            total += wave.randomSpawnRates[i].rate;
        }

        return total;
    }

    public IEnumerator SpawnRandomEnemy()
    {
        for (int i = 0; i < wave.count; i++)
        {
            RandomSpawnRate e = wave.randomSpawnRates[GetRandomEnemyIndex()];

            GameObject enemyObject = e.prefab;

            enemies.Add(enemyObject);

            Instantiate(enemyObject, wave.location.position, Quaternion.identity, transform);

            yield return new WaitForSeconds(0.3f);
        }

        wave.status = true;
    }

    public int GetWaveEnemies()
    {
        return wave.count;
    }

    public bool IsWaveSpawned()
    {
        return wave.status;
    }
}
