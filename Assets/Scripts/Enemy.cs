using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy")]
public class Enemy : ScriptableObject
{
    public int id;

    public string enemyName;

    public float damage;

    public float health;

    public float moveSpeed;

    public float range; 

    public GameObject image;

    public EnemyWithGuns gun;
}

