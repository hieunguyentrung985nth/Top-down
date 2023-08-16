//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class Inventory : MonoBehaviour 
//{
//    private ItemList itemList = new ItemList();

//    private DataToSave dataToSave = new DataToSave();
//    public static Inventory Instance { get; private set; }
//    private void Awake()
//    {
//        Instance = this;

//        itemList.weaponsList = new List<WeaponItem>();

//        itemList.supportList = new List<SupportItem>();

//        itemList.powerList = new List<PowerItem>();

//        //SaveSystem.DeleteSave();
//    }

//    public DataToSave GetItems()
//    {
//        //var data = SaveSystem.LoadPlayer();

//        //if(data != null)
//        //    this.dataToSave = data;

//        return dataToSave;
//    }

    

//    public WeaponAmmoAmount AddWeaponItem(WeaponItem itemRecevied)
//    {
//        var res = dataToSave.weaponAmount.Where(w => w.item.GetInstanceID() == itemRecevied.GetInstanceID()).ToList();

//        if (res.Count == 0)
//        {
//            WeaponAmmoAmount newWp = new WeaponAmmoAmount
//            {
//                item = itemRecevied,
//                amount = CalculateNumberToPercentage(20, itemRecevied.maxAmmo)
//            };

//            dataToSave.weaponAmount.Add(newWp);

//            var data = JsonUtility.ToJson(dataToSave);

//            Debug.Log(data);

//            PlayerPrefs.SetString("Inventory", data);

//            return newWp;
//        }
//        else
//        {
//            WeaponAmmoAmount wp = res.Find(w => w.item.GetInstanceID() == itemRecevied.GetInstanceID());

//            if(wp != null)
//            {
//                wp.amount += CalculateNumberToPercentage(10, itemRecevied.maxAmmo);

//                dataToSave.weaponAmount.Find(w => w == wp).amount = wp.amount;

//                Debug.Log(JsonUtility.ToJson(dataToSave));

//                PlayerPrefs.SetString("Inventory", JsonUtility.ToJson(dataToSave));
//            }

//            else
//            {

//            }
//            WeaponAmmoAmount newWp = new WeaponAmmoAmount
//            {
//                item = itemRecevied,
//                amount = dataToSave.weaponAmount.Find(w => w.item.GetInstanceID() == itemRecevied.GetInstanceID()).amount += CalculateNumberToPercentage(10, itemRecevied.maxAmmo)
//            };

            

//            var data = JsonUtility.ToJson(dataToSave);

//            Debug.Log(data);

//            PlayerPrefs.SetString("Inventory", data);

//            return newWp;
//        }
//    }

//    public void SaveInventory(DataToSave dataToSave)
//    {
//        //PlayerPrefs.DeleteKey("Inventory");

//        //PlayerPrefs.SetString("Inventory", JsonUtility.ToJson(dataToSave));

//        //Debug.Log(PlayerPrefs.GetString("Inventory"));

//        //SaveSystem.SavePlayer(dataToSave);

//        Debug.Log("Save successfully!");
//    }
//}
