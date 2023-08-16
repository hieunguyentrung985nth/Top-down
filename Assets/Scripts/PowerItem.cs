using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Power Item", menuName = "Items/New Power Item")]
[Serializable]
public class PowerItem : Item
{
    public int maxQuantity;

    public float effectAmount;

    public Sprite image;

    public PowerType powerType;

    private void Awake()
    {
        type = ItemType.POWER;
    }
}

public enum PowerType
{
    HP,
    SPD,
    STR,
    ACC
}
