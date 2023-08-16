using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public event Action<GameObject> OnEnterBattle;

    private GameObject spawner;

    private EnemySpawner[] spawnEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnEnterBattle?.Invoke(this.gameObject);

            spawnEnemy = this.transform.parent.GetComponents<EnemySpawner>();

            if(spawnEnemy != null)
            {
                foreach (var spawn in spawnEnemy)
                {
                    StartCoroutine(spawn.SpawnRandomEnemy());
                }

                this.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                return;
            }
        }
    }
}
