using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyToPick : MonoBehaviour
{
    [SerializeField] private MoneyItem item;

    private void Awake()
    {
        Destroy(gameObject, 45f);
    }

    public MoneyItem GetAmountMoney()
    {
        return item;
    }
}
