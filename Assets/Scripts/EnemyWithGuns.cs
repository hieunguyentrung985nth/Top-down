using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Gun")]
[Serializable]

public class EnemyWithGuns : ScriptableObject
{
    public float rangeToShoot;

    public float bulletDamage;

    public float bulletSpeed;

    public float gunHealth;

    public float fireTimeCooldown;

    public GameObject bullet;
}
