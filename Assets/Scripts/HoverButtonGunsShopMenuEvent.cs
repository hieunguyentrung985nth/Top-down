using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButtonGunsShopMenuEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ShopMenu shopMenu;

    private void Awake()
    {
        shopMenu = FindObjectOfType<ShopMenu>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerEnter.transform.parent.Find("Select").GetComponent<Image>().enabled = true;

        shopMenu.DisplayWeaponItemInfo(eventData.pointerEnter.transform.parent);

        SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.ShopButtonHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventData.pointerEnter.transform.parent.Find("Select").GetComponent<Image>().enabled = false;

        shopMenu.DisplayWeaponItemInfo();

        shopMenu.TurnOffNotEnoughMoney();
    }
}
