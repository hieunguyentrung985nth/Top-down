using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Money Item", menuName = "Items/New Money Item")]
[Serializable]
public class MoneyItem : Item
{
    public float amount;
}
