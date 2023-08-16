using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAmmo : MonoBehaviour
{
    private List<WeaponAmmoAmount> weapons = new List<WeaponAmmoAmount>();

    public event Action<WeaponItem> OnPickUpAmmoEvent;

    private void Start()
    {
       
    }
    private void Awake()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ammo"))
        {
            WeaponItem itemReceived = collision.gameObject.GetComponent<WeaponToPick>().GetItem();

            OnPickUpAmmoEvent?.Invoke(itemReceived);

            //if (weaponExist == null)
            //{
            //    weapons.Add(new WeaponAmmoAmount
            //    {
            //        item = weaponExist.item,
            //        amount = Inventory.Instance.CalculatePercentage(10, weaponExist.item.maxAmmo)
            //    });

            //}
            //else
            //{

            //}

            //Inventory.Instance.AddWeaponItem(itemReceived);

            //GetComponent<PlayerWeapon>().PopupText(itemReceived);

            //ui.UpdatePopup(itemReceived);

        }
    }
}
