using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/New Weapon")]
[Serializable]
public class WeaponItem : Item
{
    public string weaponName;

    public float damage;

    public float fireTimeCooldown;

    public int index;

    public float maxAmmo;

    public float bulletSpeed;

    public float radius;

    public float ammoPrice;

    public Sprite image;

    public GameObject prefAmmo;

    public GameObject hitVFX;

    private void Awake()
    {
        type = ItemType.WEAPON;
    }

}
