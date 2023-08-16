using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUpItem : MonoBehaviour
{   
    public event Action<WeaponItem, Collider2D> OnPickUpWeaponEvent;

    public event Action<WeaponItem, Collider2D> OnPickUpAmmoEvent;

    public event Action<HealthItem, Collider2D> OnPickUpHealthEvent;

    public event Action<PowerItem, Collider2D> OnPickUpPowerEvent;

    public event Action<float> OnPickUpMoneyEvent;

    public event Action<float> OnPickUpShieldEvent;

    private void Awake()
    {
    
    }

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            WeaponItem itemReceived = collision.gameObject.GetComponent<WeaponToPick>().GetItem();

            OnPickUpWeaponEvent?.Invoke(itemReceived, collision);
        }

        else if (collision.CompareTag("Ammo"))
        {
            WeaponItem itemReceived = collision.gameObject.GetComponent<WeaponToPick>().GetItem();

            OnPickUpAmmoEvent?.Invoke(itemReceived, collision);
        }

        else if (collision.CompareTag("Health"))
        {
            HealthItem itemReceived = collision.gameObject.GetComponent<HealthToPick>().GetItem();

            OnPickUpHealthEvent?.Invoke(itemReceived, collision);
        }

        else if (collision.CompareTag("Money"))
        {
            MoneyItem moneyReceived = collision.gameObject.GetComponent<MoneyToPick>().GetAmountMoney();

            OnPickUpMoneyEvent?.Invoke(moneyReceived.amount);

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);

            Destroy(collision.gameObject);
        }

        else if (collision.CompareTag("Item"))
        {
            float itemReceived = collision.gameObject.GetComponent<SupportToPick>().GetShieldDuration();

            OnPickUpShieldEvent?.Invoke(itemReceived);

            Destroy(collision.gameObject);
        }

        else if (collision.CompareTag("Power Item"))
        {
            PowerItem itemReceived = collision.gameObject.GetComponent<PowerToPick>().GetItem();

            OnPickUpPowerEvent?.Invoke(itemReceived, collision);
        }
    }

}


