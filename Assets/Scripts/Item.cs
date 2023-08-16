using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Item : ScriptableObject
{
    public int id;

    public string itemName;

    [TextArea(3, 10)]
    public string description;

    public float price;

    public GameObject prefab;

    public ItemType type;
}
public enum ItemType
{
    WEAPON,
    SUPPORT,
    POWER,
    HEALTH
}
