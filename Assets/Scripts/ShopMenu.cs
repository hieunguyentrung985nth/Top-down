using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    private GameManager gameManager;

    private GameObject prefPlayer;

    private Transform leftColumn;

    private Transform rightColumn;

    private List<WeaponAmmoAmount> playerWeaponItems;

    private List<SupportItemAmount> playerSupportItems;

    private List<PowerItemAmount> playerPowerItems;

    private float playerMoney;

    private List<WeaponItem> allWeaponItems;

    private List<SupportItem> allSupportItems;

    private List<PowerItem> allPowerItems;

    private Transform infoTransform;

    private Transform moneyTransform;

    [SerializeField] private Transform notEnoughMoney;

    [SerializeField] private RectTransform gunTemplate;

    private HoverButtonGunsShopMenuEvent hover;

    private bool isRenderFirstTime = true;

    private bool isNewGame;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        notEnoughMoney.gameObject.SetActive(false);

        leftColumn = transform.Find("Container").Find("Left Column");

        rightColumn = transform.Find("Container").Find("Right Column");

        infoTransform = leftColumn.Find("Info");

        moneyTransform = leftColumn.Find("Money Label");

        HandleScenes.Instance.OnHackMoney += Instance_OnHackMoney;
    }

    private void Start()
    {

        leftColumn.Find("Power Items").GetChild(0).Find("Power Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            BuyPower();
        });

        infoTransform.Find("Item Title").GetComponent<TextMeshProUGUI>().text = "";

        infoTransform.Find("Item Description").GetComponent<TextMeshProUGUI>().text = "";

        allWeaponItems = InventoryDatabase.Instance.GetWeapon.Values.ToList();

        allSupportItems = InventoryDatabase.Instance.GetSupport.Values.ToList();

        allPowerItems = InventoryDatabase.Instance.GetPower.Values.ToList();
    }

    private void OnEnable()
    {
        isNewGame = gameManager.IsNewGame();

        if (isNewGame)
        {
            playerWeaponItems = new List<WeaponAmmoAmount>();

            playerPowerItems = new List<PowerItemAmount>();

            InventoryObject.Instance.Container.weapons = playerWeaponItems;

            InventoryObject.Instance.Container.info.money = 0;

            InventoryObject.Instance.Container.power = playerPowerItems;

            playerMoney = InventoryObject.Instance.Container.info.money;
        }
        else
        {
            print("Continue");

            playerWeaponItems = InventoryObject.Instance.Container.weapons;

            playerMoney = InventoryObject.Instance.Container.info.money;

            playerPowerItems = InventoryObject.Instance.Container.power;
        }

        allPowerItems = gameManager.GetAllPowerItems();

        prefPlayer = gameManager.GetCurrentCharacterPref();

        DisplayShopMenu();
    }

    private void OnDisable()
    {
        Transform gunsContainer = rightColumn.Find("Guns Container");

        //foreach (Transform gunTransform in gunsContainer.transform)
        //{
        //    Destroy(gunTransform.gameObject);
        //}

        foreach (Button btn in gunsContainer.transform.GetComponentsInChildren<Button>())
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    private void DisplayShopMenu()
    {
        TurnOffNotEnoughMoney();

        DisplayPowers();

        DisplayMoney();

        DisplayCharacterImage();

        DisplayAllGuns();
    }

    private void DisplayMoney()
    {
        moneyTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = InventoryObject.Instance.Container.info.money.ToString() + "$";
    }

    private void DisplayPowers()
    {
        leftColumn.Find("Power Items").GetChild(0).Find("Power Money").GetComponent<TextMeshProUGUI>().text = InventoryDatabase.Instance.GetPower[0].price.ToString() + "$";       
        PowerItemAmount hpPower = playerPowerItems.Where(p => p.typePower == PowerType.HP).FirstOrDefault();

        if (hpPower != null)
        {
            for (int i = 0; i < hpPower.amount; i++)
            {
                leftColumn.Find("Power Items").GetChild(0).Find("Quantity").GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                leftColumn.Find("Power Items").GetChild(0).Find("Quantity").GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            }
        }
    }

    private void DisplayCharacterImage()
    {
        Transform imageCharacter = leftColumn.Find("Player").Find("Character Image");

        imageCharacter.GetComponent<Image>().sprite = prefPlayer.GetComponent<SpriteRenderer>().sprite;

        imageCharacter.GetComponent<Image>().color = prefPlayer.GetComponent<SpriteRenderer>().color;
    }

    private void DisplayAllGuns()
    {
        allWeaponItems = gameManager.GetAllWeaponItems();

        Transform gunsContainer = rightColumn.Find("Guns Container");

        for (int i = 0; i < allWeaponItems.Count; i++)
        {
            Transform weaponClone = null;

            if (isRenderFirstTime)
            {
                weaponClone = Instantiate(gunTemplate, gunsContainer);
            }
            else
            {
                weaponClone = gunsContainer.GetChild(i + 1);
            }

            if (i == 0)
            {
                weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().sprite = allWeaponItems[i].image;

                weaponClone.Find("Ammo").Find("Money").GetComponent<TextMeshProUGUI>().text = "";

                weaponClone.Find("Weapon").Find("Money").GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().sprite = allWeaponItems[i].image;

                weaponClone.Find("Weapon").Find("Money").GetComponent<TextMeshProUGUI>().text = allWeaponItems[i].price.ToString() + "$";

                weaponClone.Find("Weapon").Find("Ammo Slider").Find("BG Index").Find("Weapon Index").GetComponent<TextMeshProUGUI>().text = allWeaponItems[i].index.ToString();

                weaponClone.Find("Ammo").Find("Money").GetComponent<TextMeshProUGUI>().text = gameManager.CalculatePercentageToNumber(20, DataBaseTemp.Instance.maxAmmoList[allWeaponItems[i].id]) + "/" + allWeaponItems[i].ammoPrice.ToString() + "$";

                weaponClone.Find("Ammo").Find("Ammo Image").GetComponent<Image>().sprite = allWeaponItems[i].prefAmmo?.GetComponent<SpriteRenderer>().sprite;

                int x = i;

                weaponClone.Find("Weapon").Find("Weapon Button").GetComponent<Button>().onClick.AddListener(() =>
                {
                    BuyWeapon(allWeaponItems[x], weaponClone);
                });

                weaponClone.Find("Ammo").Find("Ammo Button").GetComponent<Button>().onClick.AddListener(() =>
                {
                    BuyAmmo(allWeaponItems[x], weaponClone);
                });

                if (isNewGame)
                {
                    weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().color = Color.red;

                    weaponClone.Find("Weapon").Find("Ammo Slider").GetComponent<Slider>().value = Mathf.Lerp(0, 1, 0 / 100);

                    weaponClone.Find("Weapon").Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, 0 / 100);

                    continue;
                }

                WeaponAmmoAmount wp = playerWeaponItems.Where(p => p.item.id == allWeaponItems[i].id).FirstOrDefault();

                if (wp != null)
                {
                    if (wp.status == false)
                    {
                        weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().color = Color.red;
                    }

                    else
                    {
                        weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().color = Color.white;
                    }

                    
                }
                else
                {
                    weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().color = Color.red;
                }

                var currentAmmo = gameManager.CalculateNumberToPercentage(wp != null ? wp.amount : 0, DataBaseTemp.Instance.maxAmmoList[allWeaponItems[i].id]);

                weaponClone.Find("Weapon").Find("Ammo Slider").GetComponent<Slider>().value = Mathf.Lerp(0, 1, currentAmmo / 100);

                weaponClone.Find("Weapon").Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, currentAmmo / 100);
            }
        }

        isRenderFirstTime = false;

        gunTemplate.gameObject.SetActive(false);
    }

    private void BuyWeapon(WeaponItem currentWeapon, Transform weaponClone)
    {
        WeaponAmmoAmount wp = playerWeaponItems?.Where(p => p.id == currentWeapon.id).FirstOrDefault();        

        if(wp != null && wp.status == true)
        {
            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.CantBuy);

            return;
        }

        if(InventoryObject.Instance.Container.info.money >= InventoryDatabase.Instance.GetWeapon[currentWeapon.id].price)
        {
            if (wp == null)
            {
                playerWeaponItems.Add(new WeaponAmmoAmount
                {
                    id = currentWeapon.id,

                    amount = gameManager.CalculatePercentageToNumber(20, DataBaseTemp.Instance.maxAmmoList[currentWeapon.id]),

                    item = currentWeapon,

                    status = true
                });

                wp = playerWeaponItems?.Where(p => p.id == currentWeapon.id).FirstOrDefault();

                wp.amount = Mathf.Clamp(wp.amount + gameManager.CalculatePercentageToNumber(20, DataBaseTemp.Instance.maxAmmoList[wp.item.id]), 0, DataBaseTemp.Instance.maxAmmoList[wp.item.id]);

                var currentAmmo = gameManager.CalculateNumberToPercentage(wp.amount, DataBaseTemp.Instance.maxAmmoList[wp.item.id]);

                weaponClone.Find("Weapon").Find("Ammo Slider").GetComponent<Slider>().value = Mathf.Lerp(0, 1, currentAmmo / 100);

                weaponClone.Find("Weapon").Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, currentAmmo / 100);
            }

            else
            {
                wp.status = true;
            }

            InventoryObject.Instance.Container.info.money = Mathf.Clamp(InventoryObject.Instance.Container.info.money - currentWeapon.price, 0, InventoryObject.Instance.Container.info.money);

            DisplayMoney();

            weaponClone.Find("Weapon").Find("Weapon Image").GetComponent<Image>().color = Color.white;

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);
        }
        else
        {
            DisplayNotEnoughMoney();
        }
    }

    public void TurnOffNotEnoughMoney()
    {
        notEnoughMoney.gameObject.SetActive(false);
    }

    private void BuyAmmo(WeaponItem currentWeapon, Transform weaponClone)
    {
        WeaponAmmoAmount wp = playerWeaponItems?.Where(p => p.id == currentWeapon.id).FirstOrDefault();

        if (wp != null && wp.amount == DataBaseTemp.Instance.maxAmmoList[wp.item.id])
        {
            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.CantBuy);

            return;
        }

        if (InventoryObject.Instance.Container.info.money >= InventoryDatabase.Instance.GetWeapon[currentWeapon.id].ammoPrice)
        {
            if (wp == null)
            {
                wp = new WeaponAmmoAmount
                {
                    id = currentWeapon.id,

                    amount = gameManager.CalculatePercentageToNumber(20, DataBaseTemp.Instance.maxAmmoList[currentWeapon.id]),

                    item = currentWeapon,

                    status = false
                };

                playerWeaponItems.Add(wp);
            }

            InventoryObject.Instance.Container.info.money = Mathf.Clamp(InventoryObject.Instance.Container.info.money - InventoryDatabase.Instance.GetWeapon[currentWeapon.id].ammoPrice, 0, InventoryObject.Instance.Container.info.money);

            DisplayMoney();

            wp.amount = Mathf.Clamp(wp.amount + gameManager.CalculatePercentageToNumber(20, DataBaseTemp.Instance.maxAmmoList[wp.item.id]), 0, DataBaseTemp.Instance.maxAmmoList[wp.item.id]);

            var currentAmmo = gameManager.CalculateNumberToPercentage(wp.amount, DataBaseTemp.Instance.maxAmmoList[wp.item.id]);

            weaponClone.Find("Weapon").Find("Ammo Slider").GetComponent<Slider>().value = Mathf.Lerp(0, 1, currentAmmo / 100);

            weaponClone.Find("Weapon").Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, currentAmmo / 100);

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);
        }
        else
        {
            DisplayNotEnoughMoney();
        }
    }

    private void BuyPower()
    {
        PowerItemAmount hpPower = playerPowerItems?.Where(p => p.typePower == PowerType.HP).FirstOrDefault();

        if(InventoryObject.Instance.Container.info.money >= InventoryDatabase.Instance.GetPower[0].price)
        {
            if(hpPower != null && hpPower.amount < 3)
            {
                hpPower.amount++;
            }

            else if(hpPower == null)
            {
                playerPowerItems.Add(new PowerItemAmount
                {
                    typePower = PowerType.HP,

                    amount = 1,

                    id = InventoryDatabase.Instance.GetPower[0].id,

                    item = InventoryDatabase.Instance.GetPower[0]
                });
            }

            else
            {
                return;
            }

            InventoryObject.Instance.Container.info.money = Mathf.Clamp(InventoryObject.Instance.Container.info.money - InventoryDatabase.Instance.GetPower[0].price, 0, InventoryObject.Instance.Container.info.money);

            DisplayMoney();

            DisplayPowers();

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);
        }

        else
        {
            DisplayNotEnoughMoney();
        }      
    }

    private void DisplayNotEnoughMoney()
    {
        notEnoughMoney.gameObject.SetActive(true);

        SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.CantBuy);
    }

    public void DisplayWeaponItemInfo(Transform currentHover = null)
    {
        if (currentHover == null)
        {
            infoTransform.Find("Item Title").GetComponent<TextMeshProUGUI>().text = "";

            infoTransform.Find("Item Description").GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            WeaponItem currentWeapon = allWeaponItems.Where(w => w.index == int.Parse(currentHover.parent.parent.Find("Weapon").Find("Ammo Slider").Find("BG Index").Find("Weapon Index").GetComponent<TextMeshProUGUI>().text)).FirstOrDefault();

            infoTransform.Find("Item Title").GetComponent<TextMeshProUGUI>().text = currentWeapon.weaponName;

            infoTransform.Find("Item Description").GetComponent<TextMeshProUGUI>().text = currentWeapon.description;
        }        
    }

    public void DisplayPowerItemInfo()
    {
        PowerItem currentItem = allPowerItems.Where(w => w.powerType == PowerType.HP).FirstOrDefault();

        infoTransform.Find("Item Title").GetComponent<TextMeshProUGUI>().text = currentItem.itemName;

        infoTransform.Find("Item Description").GetComponent<TextMeshProUGUI>().text = currentItem.description;
    }

    private void Instance_OnHackMoney()
    {
        DisplayMoney();
    }
}
