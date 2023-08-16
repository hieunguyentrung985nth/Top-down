using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Health Item", menuName = "Items/New Health Item")]
[Serializable]
public class HealthItem : Item
{
    public float health;

    public Sprite image;

    private void Awake()
    {
        type = ItemType.HEALTH;
    }
}
