using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private InventoryObject inventory;

    private GameManager gameManager;

    [SerializeField] private PlayerData playerData;

    //[SerializeField] private float maxHealth;

    //[SerializeField] private float moveSpeed;

    //[SerializeField] private float recoil = 1f;

    private float money;

    private List<WeaponAmmoAmount> weapons = new List<WeaponAmmoAmount>();

    private List<PowerItemAmount> powers = new List<PowerItemAmount>();

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        weapons = inventory.Container.weapons;

        money = inventory.Container.info.money;

        powers = inventory.Container.power;
    }

    public InventoryObject GetPlayerInventory()
    {
        return inventory;
    }

    public List<WeaponAmmoAmount> GetPlayerWeapons()
    {
        weapons = InventoryObject.Instance.Container.weapons;

        return weapons;
    }

    public float GetPlayerMoney()
    {
        money = InventoryObject.Instance.Container.info.money;

        return money;
    }

    public float GetDefaultHealth()
    {
        return playerData.maxHealth;
    }

    public float GetPlayerHealth()
    {
        powers = inventory.Container.power;

        DataBaseTemp.Instance.maxHealth = GetDefaultHealth();

        var hpPower = powers.Where(p => p.typePower == PowerType.HP).FirstOrDefault();

        if (hpPower != null)
        {
            for (int i = 0; i < hpPower.amount; i++)
            {
                DataBaseTemp.Instance.maxHealth += gameManager.CalculatePercentageToNumber(hpPower.item.effectAmount, DataBaseTemp.Instance.maxHealth);
            }           
        }

        return DataBaseTemp.Instance.maxHealth;
    }

    public List<PowerItemAmount> GetPlayerPowers()
    {
        powers = inventory.Container.power;

        return powers;
    }

    public bool SetPlayerHealth(float value, float currentHeal)
    {
        powers = inventory.Container.power;

        var powerItem = powers.Where(p => p.typePower == PowerType.HP).FirstOrDefault();

        float count = powerItem != null ? powerItem.amount : 0;

        if (count == 0)
        {
            //playerData.maxHealth = value;

            DataBaseTemp.Instance.maxHealth = value;

            powers.Add(new PowerItemAmount
            {
                id = InventoryDatabase.Instance.GetPower[0].id,

                amount = 1,

                typePower = PowerType.HP,

                item = InventoryDatabase.Instance.GetPower[0]
            });

            return true;
        }

        else if(count > 0 && count < 3)
        {
            //playerData.maxHealth = value;

            DataBaseTemp.Instance.maxHealth = value;

            powerItem.amount++;

            return true;
        }

        else if(currentHeal < DataBaseTemp.Instance.maxHealth && value == 0)
        {
            return true;
        }


        return false;

    }

    public float GetPlayerMoveSpeed()
    {
        return playerData.moveSpeed;
    }

    public float GetPlayerRecoil()
    {
        return playerData.recoil;
    }

    public float CalculateNumberToPercentage(float current, float total)
    {
        return Mathf.Round(current / (total / 100));
    }

    public float CalculatePercentageToNumber(float current, float total)
    {
        return Mathf.Round((total / 100) * current);
    }

    private void OnApplicationQuit()
    {
        inventory.Container.weapons.Clear();

        inventory.Container.support.Clear();

        inventory.Container.power.Clear();

        inventory.Container.info.money = 0;

        inventory.Container.info.currentLevel = 0;

        inventory.Container.info.characterIndex = 0;
    }
}
