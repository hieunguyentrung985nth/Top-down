using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Support Item", menuName = "Items/New Support Item")]
[Serializable]
public class SupportItem : Item
{
    public int maxQuantity;

    public Sprite image;

    private void Awake()
    {
        type = ItemType.SUPPORT;
    }
}
