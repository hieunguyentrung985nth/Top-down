using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponToPick : MonoBehaviour
{
    [SerializeField] private WeaponItem item;

    private void Awake()
    {
        Destroy(gameObject, 45f);
    }

    public WeaponItem GetItem()
    {
        return item;
    }
}
