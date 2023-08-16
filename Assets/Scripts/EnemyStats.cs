using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    [SerializeField] private bool hasGun;

    private void Awake()
    {
        
    }

    public Enemy GetEnemy()
    {
        return enemy;
    }

    public bool IsEnemyHasGun()
    {
        return hasGun;
    }

    public float GetHealth()
    {
        return enemy.health;
    }

    public float GetDamage()
    {
        return enemy.damage;
    }

    public float GetMoveSpeed()
    {
        return enemy.moveSpeed;
    }

    public float GetRange()
    {
        return enemy.range;
    }

    public float GetRangeToShoot()
    {
        return enemy.gun ? enemy.gun.rangeToShoot : 0;
    }

    public float GetFireTimeCoolDown()
    {
        return enemy.gun ? enemy.gun.fireTimeCooldown : 0;
    }

    public float GetGunHealth()
    {
        return enemy.gun ? enemy.gun.gunHealth : 0;
    }
}
