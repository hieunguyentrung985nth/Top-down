using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    private PickUpItem PickUpMoney;

    public event Action<string> PopUpTextEvent;

    private float currentMoney;

    private void Awake()
    {
        PickUpMoney = GetComponent<PickUpItem>();     
    }

    private void Start()
    {
        currentMoney = GetComponent<PlayerStats>().GetPlayerMoney();
    }

    private void OnEnable()
    {
        PickUpMoney.OnPickUpMoneyEvent += PickUpMoney_OnPickUpMoneyEvent;
    }

    private void OnDisable()
    {
        PickUpMoney.OnPickUpMoneyEvent -= PickUpMoney_OnPickUpMoneyEvent;
    }

    private void PickUpMoney_OnPickUpMoneyEvent(float obj)
    {
        AddMoney(obj);
    }

    void AddMoney(float amount)
    {
        currentMoney += amount;

        InventoryObject.Instance.Container.info.money += amount;

        PopUpTextEvent?.Invoke("+" + amount + " money");
    }

    public float GetPlayerMoney()
    {
        return currentMoney;
    }
}
