using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory", menuName = "Inventory/New Inventory")]
[Serializable]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public static InventoryObject Instance { get; private set; }

    private string path;

    private InventoryDatabase database;

    public InventorySlot Container = new InventorySlot();

    public DataBaseTemp tempData;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        Instance = this;

        path = Path.Combine(Application.persistentDataPath, "player.save");

        database = Resources.Load<InventoryDatabase>("Data/Database");

        //this.LoadPlayer();
    }

    public void AddItem(Item itemToAdd, float amount, bool status)
    {
        switch (itemToAdd.type)
        {
            case ItemType.WEAPON:
                var exist1 = Container.weapons.Where(w => w.item.id == itemToAdd.id).FirstOrDefault();

                if (exist1 == null)
                    Container.weapons.Add(new WeaponAmmoAmount
                    {
                        id = itemToAdd.id,
                        item = itemToAdd as WeaponItem,
                        amount = amount,
                        status = status
                    });
                else
                    exist1.amount += amount;
                break;

            case ItemType.SUPPORT:
                var exist2 = Container.support.Where(w => w.item.id == itemToAdd.id).FirstOrDefault();

                if (exist2 == null)
                    Container.support.Add(new SupportItemAmount
                    {
                        id = itemToAdd.id,
                        item = itemToAdd as SupportItem,
                        amount = amount
                    });
                else
                    exist2.amount += amount;
                break;

            case ItemType.POWER:
                var exist3 = Container.power.Where(w => w.item.id == itemToAdd.id).FirstOrDefault();

                if (exist3 == null)
                    Container.power.Add(new PowerItemAmount
                    {
                        id = itemToAdd.id,
                        item = itemToAdd as PowerItem,
                        amount = amount,
                    });
                else
                    exist3.amount += amount;
                break;

            default:
                break;
        }
    }

    public void SavePlayer(InventoryObject data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(path, FileMode.Create);

        var json = JsonUtility.ToJson(data);

        formatter.Serialize(stream, json);

        stream.Close();
    }

    public InventoryObject LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            var json = formatter.Deserialize(stream) as string;

            JsonUtility.FromJsonOverwrite(json, this);

            stream.Close();

            return this;
        }
        else
        {
            return null;
        }
    }

    public void DeleteSave()
    {
        File.Delete(path);
    }

    

    public void OnAfterDeserialize()
    {
        if(database != null && database.GetWeapon.Count > 0)
        {
            for (int i = 0; i < Container.weapons?.Count; i++)
            {
                Container.weapons[i].item = database.GetWeapon[Container.weapons[i].id];
            }
            for (int i = 0; i < Container.support?.Count; i++)
            {
                Container.support[i].item = database.GetSupport[Container.support[i].id];
            }
            for (int i = 0; i < Container.power?.Count; i++)
            {
                Container.power[i].item = database.GetPower[Container.power[i].id];
            }
        }
        
    }

    public void OnBeforeSerialize()
    {
        
    }   
}


[Serializable]
public class InventorySlot
{
    public List<WeaponAmmoAmount> weapons = new List<WeaponAmmoAmount>();

    public List<SupportItemAmount> support = new List<SupportItemAmount>();

    public List<PowerItemAmount> power = new List<PowerItemAmount>();

    public PlayerInfo info;
}

[Serializable]
public class PlayerInfo
{
    public float money;

    public int currentLevel;

    public int characterIndex;

    public bool isClearedCampaign;

}