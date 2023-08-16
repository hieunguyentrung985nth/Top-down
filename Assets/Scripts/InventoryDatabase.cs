using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Inventory/Database")]
[Serializable]
public class InventoryDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public static InventoryDatabase Instance { get; private set; }

    //public WeaponAmmoAmount[] weapons;

    //public SupportItemAmount[] support;

    //public PowerItemAmount[] power;

    public WeaponItem[] weapon;

    public SupportItem[] support;

    public PowerItem[] power;

    public HealthItem[] health;

    public MoneyItem[] money;

    public Enemy[] enemies;

    public PlayerData[] playerData;

    public List<Item> GetItem = new List<Item>();

    public Dictionary<int, WeaponItem> GetWeapon = new Dictionary<int, WeaponItem>();

    public Dictionary<int, SupportItem> GetSupport = new Dictionary<int, SupportItem>();

    public Dictionary<int, PowerItem> GetPower = new Dictionary<int, PowerItem>();

    public Dictionary<int, HealthItem> GetHealth = new Dictionary<int, HealthItem>();

    public Dictionary<int, MoneyItem> GetMoney = new Dictionary<int, MoneyItem>();

    public Dictionary<int, Enemy> GetEnemies = new Dictionary<int, Enemy>();

    public Dictionary<int, PlayerData> GetPlayer = new Dictionary<int, PlayerData>();

    public DataBaseTemp databaseTemp;


    private void OnEnable()
    {
        Instance = this;

        Instance = Resources.Load<InventoryDatabase>("Data/Database");
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < weapon?.Length; i++)
        {
            weapon[i].id = i;

            GetWeapon.Add(i, weapon[i]);

            GetItem.Add(weapon[i]);
        }

        for (int i = 0; i < support?.Length; i++)
        {
            weapon[i].id = i;

            GetSupport.Add(i, support[i]);

            GetItem.Add(support[i]);
        }

        for (int i = 0; i < power?.Length; i++)
        {
            power[i].id = i;

            GetPower.Add(i, power[i]);

            GetItem.Add(power[i]);
        }

        for (int i = 0; i < health?.Length; i++)
        {
            health[i].id = i;

            GetHealth.Add(i, health[i]);

            GetItem.Add(health[i]);
        }

        for (int i = 0; i < money?.Length; i++)
        {
            money[i].id = i;

            GetMoney.Add(i, money[i]);

            GetItem.Add(money[i]);
        }

        for (int i = 0; i < enemies?.Length; i++)
        {
            enemies[i].id = i;

            GetEnemies.Add(i, enemies[i]);
        }

        for (int i = 0; i < playerData?.Length; i++)
        {
            playerData[i].id = i;

            GetPlayer.Add(i, playerData[i]);
        }

    }

    public void OnBeforeSerialize()
    {
        GetWeapon = new Dictionary<int, WeaponItem>();

        GetSupport = new Dictionary<int, SupportItem>();

        GetPower = new Dictionary<int, PowerItem>();

        GetItem = new List<Item>();

        GetEnemies = new Dictionary<int, Enemy>();
       
        GetPlayer = new Dictionary<int, PlayerData>();
    }
}
