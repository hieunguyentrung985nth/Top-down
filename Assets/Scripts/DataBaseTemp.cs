using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataBaseTemp
{
    public DataBaseTemp() { }

    private static DataBaseTemp _instance = new DataBaseTemp();

    public static DataBaseTemp Instance => _instance;

    public Dictionary<int, float> maxAmmoList = new Dictionary<int, float>();

    public float maxHealth;


    public void Initialize(int characterIndex)
    {
        maxAmmoList = new Dictionary<int, float>();

        PlayerData playerData = InventoryDatabase.Instance.GetPlayer[characterIndex];

        foreach (WeaponItem wp in InventoryDatabase.Instance.GetWeapon.Values)
        {
            int key = wp.id;

            float value = CalculatePercentageToNumber(playerData.strength, wp.maxAmmo);

            maxAmmoList.Add(key, value);
        }

        maxHealth = playerData.maxHealth;
    }
    public float CalculateNumberToPercentage(float current, float total)
    {
        return Mathf.Round(current / (total / 100));
    }

    public float CalculatePercentageToNumber(float current, float total)
    {
        return Mathf.Round((total / 100) * current);
    }
}
